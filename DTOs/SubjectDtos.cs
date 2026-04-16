namespace grad.DTOs
{
    // ─────────────────────────────────────────────────────────────────────────
    // Page 3 – "My Subjects" list
    // GET /api/subject/my-subjects
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// One card on the Subject-list screen (Page 3).
    /// Progress is calculated live: completed sessions / total sessions × 100.
    /// </summary>
    public class SubjectListItemDto
    {
        /// <summary>Course primary key – used to navigate to Page 4.</summary>
        public int CourseId { get; set; }

        public string Title { get; set; } = string.Empty;

        /// <summary>e.g. "Primary", "Preparatory", "Secondary"</summary>
        public string AcademicLevel { get; set; } = string.Empty;

        public int AcademicYear { get; set; }

        public string TeacherName { get; set; } = string.Empty;

        /// <summary>Teacher's subject/specialization (Teacher.subject field).</summary>
        public string TeacherSubject { get; set; } = string.Empty;

        /// <summary>0–100, derived from LessonProgress rows for this student + course.</summary>
        public int ProgressPercent { get; set; }

        /// <summary>Total number of sessions in this course.</summary>
        public int TotalSessions { get; set; }

        /// <summary>Number of sessions the student has viewed at least once.</summary>
        public int CompletedSessions { get; set; }

        public DateTime EnrolledAt { get; set; }
    }

    // ─────────────────────────────────────────────────────────────────────────
    // Page 4 – "Course Sessions" detail
    // GET /api/subject/{courseId}/sessions
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Top-level response for Page 4 – course header + ordered session list.
    /// </summary>
    public class CourseSessionsResponseDto
    {
        public int CourseId { get; set; }

        public string CourseTitle { get; set; } = string.Empty;

        public string AcademicLevel { get; set; } = string.Empty;

        public int AcademicYear { get; set; }

        public string TeacherName { get; set; } = string.Empty;

        public string TeacherSubject { get; set; } = string.Empty;

        /// <summary>Overall course progress (0–100) for this student.</summary>
        public int ProgressPercent { get; set; }

        public int TotalSessions { get; set; }

        public int CompletedSessions { get; set; }

        public IEnumerable<SessionItemDto> Sessions { get; set; } = new List<SessionItemDto>();
    }

    /// <summary>
    /// A single session row on Page 4.
    /// </summary>
    public class SessionItemDto
    {
        public int SessionId { get; set; }

        public string Title { get; set; } = string.Empty;

        /// <summary>How many days this session remains accessible after first view.</summary>
        public int AvailableDays { get; set; }

        /// <summary>Maximum allowed views configured for this session.</summary>
        public int MaxViews { get; set; }

        /// <summary>How many times THIS student has already watched the session.</summary>
        public int ViewsUsed { get; set; }

        /// <summary>
        /// Remaining views for the student (MaxViews – ViewsUsed).
        /// Clamped to 0 so it never goes negative.
        /// </summary>
        public int ViewsRemaining { get; set; }

        /// <summary>True when the student has viewed this session at least once.</summary>
        public bool IsWatched { get; set; }

        /// <summary>0–100 watch-progress percentage stored in LessonProgress.</summary>
        public double WatchProgressPercent { get; set; }

        /// <summary>Whether the session has a downloadable attachment.</summary>
        public bool HasAttachment { get; set; }

        public string? AttachmentUrl { get; set; }

        /// <summary>Whether the session has homework to submit.</summary>
        public bool HasHomework { get; set; }

        public string? HomeworkUrl { get; set; }

        /// <summary>Whether the session requires an entry quiz before watching.</summary>
        public bool HasEntryTest { get; set; }

        /// <summary>
        /// If HasEntryTest is true: has the student passed it?
        /// Null when there is no entry test.
        /// </summary>
        public bool? EntryTestPassed { get; set; }

        /// <summary>
        /// Best score the student achieved on this session's entry test (0–100).
        /// Null when there is no entry test or the student hasn't attempted it yet.
        /// </summary>
        public decimal? EntryTestBestScore { get; set; }

        /// <summary>
        /// True when the student has submitted homework for this session.
        /// </summary>
        public bool HomeworkSubmitted { get; set; }

        /// <summary>
        /// Grade awarded by the teacher after reviewing the submission.
        /// Null when not yet reviewed.
        /// </summary>
        public string? HomeworkGrade { get; set; }
    }
}
