using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Doctor.Controllers
{
    [AllowAnonymous]
    [ApiController]
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly UserManager<API.DTOs.Model.Doctor> _userManager;
        private readonly SignInManager<API.DTOs.Model.Doctor> _signInManager;
        private readonly TokenService _tokenService;
        public AccountController(TokenService tokenService, DataContext context, UserManager<API.DTOs.Model.Doctor> userManager, SignInManager<API.DTOs.Model.Doctor> signInManager)
        {
            this._tokenService = tokenService;
            this._signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult<DoctorDTO>> Login(LoginDTO user)
        {
            var doctor = await this._userManager.FindByEmailAsync(user.Email);
            if (doctor == null)
            {
                return BadRequest("No such email exist..!");
            }
            var res = await this._signInManager.CheckPasswordSignInAsync(doctor, user.Password, false);
            if (res.Succeeded)
            {
                return new DoctorDTO
                {
                    UserName = doctor.UserName.Replace(" ", String.Empty),
                    Name = doctor.Name,
                    DoctorId = doctor.DoctorId,
                    Token = _tokenService.CreateToken(doctor)
                };
            }
            else
            {
                return Unauthorized("Wrong credentials..!");
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<DoctorDTO>> Register(RegisterDTO user)
        {
            var res = await this._userManager.FindByEmailAsync(user.Email);
            if (res != null)
            {
                return BadRequest("Email already present please login..");
            }
            var doc = new API.DTOs.Model.Doctor
            {
                Name = user.UserName,
                UserName = user.UserName.Replace(" ", String.Empty),
                DoctorId = user.DoctorId,
                Email = user.Email
            };
            var result = await this._userManager.CreateAsync(doc, user.Password);
            if (result.Succeeded)
            {
                return new DoctorDTO
                {
                    UserName = doc.UserName,
                    Name = doc.Name,
                    DoctorId = doc.DoctorId,
                    Token = _tokenService.CreateToken(doc)
                };
            }
            return BadRequest("Failed to register..!");
        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<DoctorDTO>> GetCurrentUser(){
            var doc = await this._userManager.Users.FirstOrDefaultAsync(x=> x.Email == User.FindFirstValue(ClaimTypes.Email));
            if(doc==null){
                return BadRequest("No logged in user +"+User.FindFirstValue(ClaimTypes.Email));
            }
            return new DoctorDTO
                {
                    UserName = doc.UserName,
                    Name = doc.Name,
                    DoctorId = doc.DoctorId,
                    Token = _tokenService.CreateToken(doc)
                };
        }

    }
}
