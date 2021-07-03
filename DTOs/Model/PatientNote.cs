using System;
using System.Collections.Generic;

namespace API.DTOs.Model
{
    public class PatientNote
    {
         public Guid Id {get ;set;}
         public string Notes { get; set; }
         public string Remarks { get; set; }
         public string Symptoms { get; set; }
         public DateTime Date {get; set; }
    }
}