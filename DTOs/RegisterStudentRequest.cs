namespace grad.DTOs
{
    public class RegisterStudentRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? AcademicLevel { get; set; } // عشان يستقبل "Primary" مثلاً
        public int AcademicYear { get; set; }     // عشان يستقبل "1" مثلاً        public string ParentEmail { get; set; }
        public string ParentEmail { get; set; }
        public string LanguagePref { get; set; }
        public string DeviceId { get; set; }
    }
}
