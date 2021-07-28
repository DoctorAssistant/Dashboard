using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.DTOs;
using API.DTOs.Model;
using Doctor.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class DoctorController : BaseApiController
    {
        private readonly UserManager<API.DTOs.Model.Doctor> _userManager;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DoctorController(UserManager<API.DTOs.Model.Doctor> userManager, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._context = context;
            this._userManager = userManager;

        }

        [HttpGet("get-notes/{id}")]
        public async Task<ActionResult<IEnumerable<PatientNote>>> GetPatientDetails(Guid id)
        {
           
            var doctor = await this._userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            if (doctor == null)
            {
                return Unauthorized();
            }
            var patient = await this._context.Patients.Include(x => x.Notes).FirstOrDefaultAsync(x => x.Id == id);
            if (patient == null)
            {
                return BadRequest("No such patient...");
            }
            return patient.Notes.ToList();
        }

        [HttpPost("add-note/{id}")]
        public async Task<ActionResult<IEnumerable<PatientNote>>> AddNote(PatientNoteDTO note, Guid id)
        {
            var doctor = await this._userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            if (doctor == null)
            {
                return Unauthorized();
            }
            var patient = await this._context.Patients.Include(x => x.Notes).FirstOrDefaultAsync(x => x.Id == id);
            if (patient == null)
            {
                return BadRequest("No such patient...");
            }
            var newNote = new PatientNote
            {
                DoctorId = doctor.Id,
                Symptoms = note.Symptoms,
                Date = DateTime.Now,
                Notes = note.Notes,
                Remarks = note.Remarks ?? ""
            };
            patient.LastVisited = DateTime.Now;
            patient.Notes.Add(newNote);
            await _context.SaveChangesAsync();

            return patient.Notes.ToList();
        }

        [HttpPut("update-note/{id}")]
        public async Task<ActionResult<string>> UpdateNote(Guid id, PatientNoteDTO note)
        {
            var doctor = await this._userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            if (doctor == null)
            {
                return Unauthorized();
            }
            var patient = await this._context.Patients.Include(x => x.Notes).FirstOrDefaultAsync(x => x.Id == id);
            if (patient == null)
            {
                return BadRequest("No such patient...");
            }
            var noteId = Guid.Parse(this._httpContextAccessor.HttpContext.Request.Query["note-id"].ToString());
            var pNote = patient.Notes.FirstOrDefault(x => x.Id == noteId);

            pNote.Symptoms = note.Symptoms ?? pNote.Symptoms;
            pNote.Notes = note.Notes ?? pNote.Notes;
            pNote.Remarks = note.Remarks ?? pNote.Remarks;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("activity")]
        public async Task<ActionResult<IEnumerable<Patient>>> GetActivityOfDate(){
            var doctor = await this._userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            if (doctor == null)
            {
                return Unauthorized();
            }
            var dateParams = DateTime.Parse(this._httpContextAccessor.HttpContext.Request.Query["date"]);
            var patients = await this._context.Patients.Include(x => x.Notes).Where(x=> x.DoctorId == doctor.Id && x.LastVisited.Day == dateParams.Day).ToListAsync();
            if (patients == null)
            {
                return BadRequest("No patients...");
            }
            return patients; 
        }

        [HttpGet("activity/all")]
        public async Task<ActionResult<IEnumerable<Patient>>> GetAllActivity(){
            var doctor = await this._userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            if (doctor == null)
            {
                return Unauthorized();
            }
            var patients = await this._context.Patients.Include(x => x.Notes).Where(x=> x.DoctorId == doctor.Id).ToListAsync();
            if (patients == null)
            {
                return BadRequest("No patients...");
            }
            return patients;  
        }

        [HttpDelete("delete-note/{id}")]
        public async Task<ActionResult<string>> DeleteNote(Guid id)
        {
            var doctor = await this._userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            if (doctor == null)
            {
                return Unauthorized();
            }
            var patient = await this._context.Patients.Include(x => x.Notes).FirstOrDefaultAsync(x => x.Id == id);
            if (patient == null)
            {
                return BadRequest("No such patient...");
            }
            var noteId = Guid.Parse(this._httpContextAccessor.HttpContext.Request.Query["note-id"].ToString());
            var pNote = patient.Notes.FirstOrDefault(x => x.Id == noteId);
            patient.Notes.Remove(pNote);
            await _context.SaveChangesAsync();

            return "Note deleted";
        }


    }
}