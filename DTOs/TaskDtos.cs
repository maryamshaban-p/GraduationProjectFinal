using System.ComponentModel.DataAnnotations;

namespace grad.DTOs
{
    // ══════════════════════════════════════════════════════════════════════════
    // MY COURSE TAB  –  GET /api/student/tasks/my-courses
    // Matches the left screen ("My Course" tab with Ongoing / Upcoming / Completed)
    // ══════════════════════════════════════════════════════════════════════════

    public class MyCourseTabResponseDto
    {
        public IEnumerable<CourseCardDto> Ongoing { get; set; } = new List<CourseCardDto>();
        public IEnumerable<CourseCardDto> Upcoming { get; set; } = new List<CourseCardDto>();
        public IEnumerable<CourseCardDto> Completed { get; set; } = new List<CourseCardDto>();
    }

    /// <summary>Single course card shown inside the "My Course" tab.</summary>
    public class CourseCardDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        /// <summary>Time label shown below teacher name (e.g. "10:30am").</summary>
        public string? ScheduleLabel { get; set; }
        public int ProgressPercent { get; set; }
        public int TotalSessions { get; set; }
        public int CompletedSessions { get; set; }
        public string AcademicLevel { get; set; } = string.Empty;
        public int AcademicYear { get; set; }
        public DateTime EnrolledAt { get; set; }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // ADD TASK  –  POST /api/student/tasks
    // Matches the middle "Add task" screen
    // ══════════════════════════════════════════════════════════════════════════

    public class CreateTaskDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        /// <summary>The date part of the scheduled task (ISO 8601 date string).</summary>
        [Required]
        public DateTime Date { get; set; }

        /// <summary>Start time in "HH:mm" format (24h). e.g. "20:30" for 8:30 PM.</summary>
        public string? StartTime { get; set; }

        /// <summary>End time in "HH:mm" format (24h). e.g. "22:00" for 10:00 PM.</summary>
        public string? EndTime { get; set; }

        // ── Recurrence (matches the "Repeat" toggle + day picker + "Every" controls) ──

        public bool IsRecurring { get; set; } = false;

        /// <summary>"Daily" | "Weekly" | "Monthly"</summary>
        public string? RecurrenceFrequency { get; set; }

        /// <summary>The "1" in "Every 1 Weekly". Defaults to 1.</summary>
        public int RecurrenceInterval { get; set; } = 1;

        /// <summary>
        /// Active days for Weekly recurrence: comma-separated short names.
        /// e.g. "Su,Mo,Fr" – matches the S/M/T/W/T/F/S day-picker buttons.
        /// </summary>
        public string? RecurringDays { get; set; }
    }

    public class UpdateTaskDto
    {
        [MaxLength(200)]
        public string? Title { get; set; }

        public string? Description { get; set; }
        public DateTime? Date { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public bool? IsRecurring { get; set; }
        public string? RecurrenceFrequency { get; set; }
        public int? RecurrenceInterval { get; set; }
        public string? RecurringDays { get; set; }
        public bool? IsCompleted { get; set; }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // TASK RESPONSE  –  GET /api/student/tasks  |  GET /api/student/tasks/{id}
    // Matches the right "my task" screen card list
    // ══════════════════════════════════════════════════════════════════════════

    public class TaskResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public DateTime Date { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }

        public bool IsRecurring { get; set; }
        /// <summary>"Daily" | "Weekly" | "Monthly" | null</summary>
        public string? RecurrenceFrequency { get; set; }
        public int RecurrenceInterval { get; set; }
        /// <summary>e.g. "Su,Mo,Fr"</summary>
        public string? RecurringDays { get; set; }

        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // HOMEWORK CALENDAR  –  GET /api/student/tasks/homework-calendar
    //                       GET /api/student/tasks/homework-calendar/{year}/{month}
    // Matches the right "Homework" calendar screen
    // ══════════════════════════════════════════════════════════════════════════

    public class HomeworkCalendarResponseDto
    {
        public int Year { get; set; }
        public int Month { get; set; }

        /// <summary>Days in this month that have at least one homework submission.</summary>
        public IEnumerable<int> DaysWithSubmissions { get; set; } = new List<int>();

        /// <summary>All tasks falling in this month (personal tasks).</summary>
        public IEnumerable<TaskResponseDto> TasksThisMonth { get; set; } = new List<TaskResponseDto>();

        /// <summary>All homework submissions in this month.</summary>
        public IEnumerable<HomeworkCalendarItemDto> Submissions { get; set; } = new List<HomeworkCalendarItemDto>();
    }

    public class HomeworkCalendarItemDto
    {
        public int SubmissionId { get; set; }
        public string SessionTitle { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public bool IsReviewed { get; set; }
        public string? Grade { get; set; }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // TASKS BY DATE  –  GET /api/student/tasks/by-date?date=2026-09-09
    // Used when the user taps a calendar day
    // ══════════════════════════════════════════════════════════════════════════

    public class TasksByDateResponseDto
    {
        public DateTime Date { get; set; }
        public IEnumerable<TaskResponseDto> Tasks { get; set; } = new List<TaskResponseDto>();
        public IEnumerable<HomeworkCalendarItemDto> Homeworks { get; set; } = new List<HomeworkCalendarItemDto>();
    }
}
