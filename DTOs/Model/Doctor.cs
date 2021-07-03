using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace API.DTOs.Model{
    public class Doctor :  IdentityUser{
        public string Name { get; set; }
        public string DoctorId { get; set; }
        public int Age { get; set; }
        public ICollection<Patient> Patients { get; set; } =  new List<Patient>();
    }
}