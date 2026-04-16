using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace grad.Models
{
    /// <summary>
    /// Junction table for the many-to-many relationship between Students and Teachers.
    /// </summary>
    public class StudentTeacher
    {
        [Key]
        public int Id { get; set; }

        public Guid StudentId { get; set; }
        public Guid TeacherId { get; set; }

        [ForeignKey("StudentId")]
        public Student Student { get; set; } = null!;

        [ForeignKey("TeacherId")]
        public Teacher Teacher { get; set; } = null!;
    }
}
