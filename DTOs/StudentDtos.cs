namespace grad.DTOs
{
    // ══════════════════════════════════════════════════════════════════════════
    // HOME PAGE  –  GET /api/student/home
    // ══════════════════════════════════════════════════════════════════════════

    public class StudentHomeResponseDto
    {
        public string StudentName { get; set; } = string.Empty;
        public string AcademicLevel { get; set; } = string.Empty;
        public int UnreadNotifications { get; set; }

        public StudentDashboardStatsDto Statistics { get; set; } = new();

        /// <summary>Up to 5 most-recently-enrolled courses with live progress.</summary>
        public IEnumerable<RecentCourseDto> RecentCourses { get; set; } = new List<RecentCourseDto>();
    }

    public class StudentDashboardStatsDto
    {
        public int Absence { get; set; }
        public int TasksSubmitted { get; set; }
        public int QuizzesTaken { get; set; }
        /// <summary>Average progress across all enrolled courses (0-100).</summary>
        public int OverallProgress { get; set; }
    }

    public class RecentCourseDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public int ProgressPercent { get; set; }
        public int TotalSessions { get; set; }
        public int CompletedSessions { get; set; }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // NOTIFICATIONS  –  GET /api/student/notifications
    // ══════════════════════════════════════════════════════════════════════════

    public class NotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        /// <summary>"quiz" | "homework" | "CourseSession" | "general"</summary>
        public string Type { get; set; } = "general";
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // MESSAGES  –  GET /api/student/messages
    //             GET /api/student/messages/{partnerId}
    //             POST /api/student/messages/{receiverId}
    // ══════════════════════════════════════════════════════════════════════════

    public class ConversationDto
    {
        public Guid PartnerId { get; set; }
        public string PartnerName { get; set; } = string.Empty;
        public string LastMessage { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public int UnreadCount { get; set; }
    }

    public class MessageDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsMine { get; set; }
        public bool IsRead { get; set; }
    }

    public class SendMessageDto
    {
        public string Content { get; set; } = string.Empty;
    }

    // ══════════════════════════════════════════════════════════════════════════
    // PROFILE  –  GET /api/student/profile
    //             PUT /api/student/profile
    // ══════════════════════════════════════════════════════════════════════════

    public class StudentProfileDto
    {
        public Guid UserId { get; set; }
        public Guid StudentId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? LanguagePref { get; set; }
        public string? AcademicLevel { get; set; }
        public string? ClassLevel { get; set; }
        public string? ParentEmail { get; set; }
    }

    public class UpdateStudentProfileDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? LanguagePref { get; set; }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // QUIZ  –  GET  /api/student/quiz/{sessionId}
    //          POST /api/student/quiz/{sessionId}/submit
    // ══════════════════════════════════════════════════════════════════════════

    public class QuizDetailsDto
    {
        public int QuizId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int PassingScore { get; set; }
        public int RetakeIntervalHours { get; set; }
        public bool AlreadyPassed { get; set; }
        /// <summary>Null when the student has never attempted this quiz.</summary>
        public decimal? LastScore { get; set; }
        /// <summary>Null until the retake window has expired.</summary>
        public DateTime? CanRetakeAt { get; set; }
        public IEnumerable<QuizQuestionDto> Questions { get; set; } = new List<QuizQuestionDto>();
    }

    public class QuizQuestionDto
    {
        public int QuestionId { get; set; }
        public string Text { get; set; } = string.Empty;
        public IEnumerable<QuizOptionDto> Options { get; set; } = new List<QuizOptionDto>();
    }

    public class QuizOptionDto
    {
        public int OptionId { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    public class SubmitQuizDto
    {
        /// <summary>Map of questionId → selected optionId.</summary>
        public Dictionary<int, int> Answers { get; set; } = new();
    }

    public class QuizResultDto
    {
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public decimal Percentage { get; set; }
        public bool Passed { get; set; }
        public IEnumerable<QuizBreakdownItemDto> Breakdown { get; set; } = new List<QuizBreakdownItemDto>();
    }

    public class QuizBreakdownItemDto
    {
        public int QuestionId { get; set; }
        public int? SelectedOptionId { get; set; }
        public int? CorrectOptionId { get; set; }
        public bool IsCorrect { get; set; }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // HOMEWORK  –  POST /api/student/homework/{sessionId}/submit
    //              GET  /api/student/homework/{sessionId}
    // ══════════════════════════════════════════════════════════════════════════

    public class SubmitHomeworkDto
    {
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
    }

    public class HomeworkStatusDto
    {
        public int SubmissionId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public bool IsReviewed { get; set; }
        public string? Grade { get; set; }
        public string? TeacherComment { get; set; }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // VIEW-COUNT REQUEST  –  POST /api/student/request/views
    //                        POST /api/student/request/retake
    // ══════════════════════════════════════════════════════════════════════════

    public class StudentRequestDto
    {
        public int SessionId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    // ══════════════════════════════════════════════════════════════════════════
    // LESSON PROGRESS  –  POST /api/student/lesson/{sessionId}/progress
    // ══════════════════════════════════════════════════════════════════════════

    public class UpdateLessonProgressDto
    {
        /// <summary>Watch-progress percentage (0-100).</summary>
        public double ProgressPercent { get; set; }
    }

    // ══════════════════════════════════════════════════════════════════════════
    // ACHIEVEMENT / STATISTICS  –  GET /api/student/achievement
    // ══════════════════════════════════════════════════════════════════════════

    public class AchievementDto
    {
        public int TotalEnrolled { get; set; }
        public int CompletedCourses { get; set; }
        public decimal AverageQuizScore { get; set; }
        public int TotalAbsences { get; set; }
        public int TotalHomeworkSubmitted { get; set; }
        public int TotalQuizzesTaken { get; set; }
        public IEnumerable<CourseActivityDto> CoursesActivity { get; set; } = new List<CourseActivityDto>();
    }

    public class CourseActivityDto
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public int ProgressPercent { get; set; }
        public DateTime EnrolledAt { get; set; }
    }
}
