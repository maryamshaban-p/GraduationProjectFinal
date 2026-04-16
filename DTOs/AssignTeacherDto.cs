using System.ComponentModel.DataAnnotations;

namespace grad.DTOs
{
    /// <summary>
    /// Used when re-assigning / updating teachers for an existing student.
    /// </summary>
    public class AssignTeacherDto
    {
        [Required]
        public List<Guid> TeacherIds { get; set; } = new();
    }
}
