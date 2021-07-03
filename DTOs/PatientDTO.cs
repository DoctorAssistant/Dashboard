using System;

namespace API.DTOs
{
    public class PatientDTO
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
        public string Dob { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }
}