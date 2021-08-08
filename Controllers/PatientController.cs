using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using API.DTOs;
using API.DTOs.Model;
using API.Services;
using Doctor.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    public class PatientController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly UserManager<API.DTOs.Model.Doctor> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PatientController(DataContext context, UserManager<API.DTOs.Model.Doctor> userManager, IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._userManager = userManager;
            this._context = context;
        }

        [HttpGet("patients")]
        public async Task<ActionResult<IEnumerable<Patient>>> GetAllPatient()
        {
            var doctor = await this._userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            if (doctor == null)
            {
                return Unauthorized();
            }
            var patients = await this._context.Doctors.Include(x=> x.Patients).FirstOrDefaultAsync(x=> x.Id==doctor.Id);
            
            return patients.Patients.ToList();
        }

        [HttpPost("add-patient")]
        public async Task<ActionResult<Patient>> AddPatient(PatientDTO patient)
        {
            var doctor = await this._userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            if (doctor == null)
            {
                return Unauthorized();
            }
            var pa = await this._context.Patients.FirstOrDefaultAsync(x => x.Phone == patient.Phone && x.DoctorId== doctor.Id);
            if (pa != null)
            {
                if ((patient.Name == pa.Name && patient.Phone == pa.Phone) || patient.Dob == pa.Dob)
                {
                    return BadRequest("Patient with same Name, Phone and Dob exist");
                }
            }
            var newPatient = new Patient
            {
                Name = patient.Name,
                Age = patient.Age,
                Dob = patient.Dob,
                Sex = patient.Sex,
                Address = patient.Address,
                Phone = patient.Phone,
                DoctorId = doctor.DoctorId,
                LastVisited = DateTime.Now
            };
            await this._context.Patients.AddAsync(newPatient);
            doctor.Patients.Add(newPatient);
            await this._context.SaveChangesAsync();
            return newPatient;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatient(Guid id)
        {
            var doctor = await this._userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            if (doctor == null)
            {
                return Unauthorized();
            }
            var patient = await this._context.Patients.FirstOrDefaultAsync(x => x.Id == id);
            if (patient == null)
            {
                return BadRequest("No such patient exist, add patient...");
            }
            return patient;
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<Patient>>> SearchPatient()
        {
            var name = _httpContextAccessor.HttpContext.Request.Query["name"].ToString();
            var doctor = await this._userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            if (doctor == null)
            {
                return Unauthorized();
            }
            var patients = await _context.Patients.Where(p => p.Name.Contains(name)).ToListAsync();
            return patients;
        }

        [HttpPut("update-patient/{id}")]
        public async Task<ActionResult<Patient>> UpdatePatient(Patient patient, Guid id)
        {
            var doctor = await this._userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            if (doctor == null)
            {
                return Unauthorized();
            }
            var pa = await _context.Patients.FirstOrDefaultAsync(x => x.Id == id);
            if (pa == null)
            {
                return BadRequest("Patient data doesn't exists..");
            }

            pa.Name = patient.Name ?? pa.Name;
            pa.Age = patient.Age > 0 ? patient.Age : pa.Age;
            pa.Dob = patient.Dob ?? pa.Dob;
            pa.Sex = patient.Sex ?? pa.Sex;
            pa.Address = patient.Address ?? pa.Address;
            pa.Phone = patient.Phone ?? pa.Phone;
            pa.DoctorId = patient.DoctorId ?? pa.DoctorId;
            pa.LastVisited = DateTime.Now;

            await _context.SaveChangesAsync();
            return pa;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeletePatient(Guid id)
        {
            var doctor = await this._userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));
            if (doctor == null)
            {
                return Unauthorized();
            }
            var pa = await _context.Patients.FirstOrDefaultAsync(x => x.Id == id);
            if (pa == null)
            {
                return BadRequest("Patient data doesn't exists..");
            }
            _context.Patients.Remove(pa);
            await _context.SaveChangesAsync();
            return "Data Deleted";
        }


    }
}