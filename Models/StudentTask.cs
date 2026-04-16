using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace grad.Models
{
    /// <summary>
    /// A personal task/reminder created by a student.
    /// Supports one-time and recurring schedules (Daily / Weekly / Monthly).
    /// Matches the "Add task" and "My Task" screens in the mobile UI.
    /// </summary>
    public class StudentTask
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Student")]
        public Guid StudentId { get; set; }
        public Student Student { get; set; } = null!;

        // ── Core fields ──────────────────────────────────────────────────────

        /// <summary>Short title shown on the task card (e.g. "MeetWithClass").</summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>Optional longer description.</summary>
        public string? Description { get; set; }

        // ── Schedule ─────────────────────────────────────────────────────────

        /// <summary>The date this task is scheduled for (or the first occurrence when recurring).</summary>
        public DateTime Date { get; set; }

        /// <summary>Start time of day (stored as UTC offset from midnight, displayed locally).</summary>
        public TimeSpan? StartTime { get; set; }

        /// <summary>End time of day.</summary>
        public TimeSpan? EndTime { get; set; }

        // ── Recurrence ───────────────────────────────────────────────────────

        /// <summary>Whether recurrence is enabled.</summary>
        public bool IsRecurring { get; set; } = false;

        /// <summary>
        /// Recurrence frequency unit: "Daily" | "Weekly" | "Monthly".
        /// Null when IsRecurring = false.
        /// </summary>
        [MaxLength(20)]
        public string? RecurrenceFrequency { get; set; }

        /// <summary>
        /// "Every N weeks/months" interval.
        /// e.g. 1 = every week, 2 = every 2 weeks.
        /// </summary>
        public int RecurrenceInterval { get; set; } = 1;

        /// <summary>
        /// Comma-separated days of the week for Weekly recurrence.
        /// Values: "Su,Mo,Tu,We,Th,Fr,Sa"  – matches the day-picker in the UI.
        /// Null for Daily and Monthly frequencies.
        /// </summary>
        [MaxLength(50)]
        public string? RecurringDays { get; set; }

        // ── Status ───────────────────────────────────────────────────────────

        public bool IsCompleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
