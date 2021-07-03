using System;
using System.Collections.Generic;

namespace API.DTOs.Model{
    public class Patient{
        public Guid Id {get ;set;}
        public string Name { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
        public DateTime? Dob { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public ICollection<PatientNote> Notes {get; set;} = new List<PatientNote>();
    }
}