namespace grad.DTOs
{
    /// <summary>
    /// Auto-computed statistics returned whenever a student opens their dashboard.
    /// All counts are derived live from the database — no manual moderator input needed.
    /// </summary>
    public class StudentStatisticsDto
    {
        /// <summary>The student's unique identifier.</summary>
        public Guid StudentId { get; set; }

        /// <summary>
        /// Total number of absence records in the StudentAbsences table for this student.
        /// Returns 0 when no records exist.
        /// </summary>
        public int Absence { get; set; }

        /// <summary>
        /// Total number of homework submissions in the HomeworkSubmissions table.
        /// Represents tasks the student has submitted.  Returns 0 when none exist.
        /// </summary>
        public int Tasks { get; set; }

        /// <summary>
        /// Total number of quiz attempts in the StudentQuizResults table.
        /// Returns 0 when no quizzes have been taken.
        /// </summary>
        public int Quiz { get; set; }
    }
}
