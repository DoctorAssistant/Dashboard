namespace API.DTOs
{
    public class RegisterDTO
    {
        public string Email {get; set;}
        public string Password {get; set;}
        public string UserName { get; set; }
        public string DoctorId { get; set; }
    }
}