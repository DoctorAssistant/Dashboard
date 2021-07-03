using System;

namespace API.DTOs
{
    public class PatientNoteDTO
    {
         public string Notes { get; set; }
         public string Remarks { get; set; }
         public string Symptoms { get; set; }
         public DateTime Date {get; set; } 
    }
}