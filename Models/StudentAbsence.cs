using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace grad.Models
{
    /// <summary>
    /// Records a single absence event for a student.
    /// One row = one absence session.  Count rows per StudentId to get the total.
    /// </summary>
    public class StudentAbsence
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Student")]
        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;

        /// <summary>Date the absence occurred (date part only).</summary>
        public DateTime AbsenceDate { get; set; } = DateTime.UtcNow;

        /// <summary>Optional note by the moderator (e.g., "sick leave").</summary>
        public string? Note { get; set; }

        /// <summary>Who recorded this absence (moderator user ID).</summary>
        public Guid? RecordedBy { get; set; }
    }
}
