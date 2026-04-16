using grad.Data;
using grad.DTOs;
using grad.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace grad.Controllers
{
    /// <summary>
    /// Handles the student-facing Subject screens:
    ///   • Page 3 – "My Subjects" list with per-course progress
    ///   • Page 4 – "Course Sessions" detail with per-session progress
    ///
    /// All progress figures are derived live from the database via LINQ.
    /// No stored/cached progress columns are read – only LessonProgress,
    /// HomeworkSubmissions, and LessonAttempts are queried.
    /// </summary>
    [ApiController]
    [Route("api/subject")]
    [Authorize(Roles = "Student")]
    public class SubjectController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public SubjectController(AppDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db          = db;
            _userManager = userManager;
        }

        // ─────────────────────────────────────────────────────────────────
        // Helper – resolve the JWT sub-claim → Student row
        // ─────────────────────────────────────────────────────────────────
        private async Task<Student?> GetCurrentStudentAsync()
        {
            var rawId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (rawId == null || !Guid.TryParse(rawId, out var userId))
                return null;

            return await _db.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.user_id == userId);
        }

        // ─────────────────────────────────────────────────────────────────
        // PAGE 3 – Subject list
        // GET /api/subject/my-subjects
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns every course the authenticated student is enrolled in,
        /// together with live-computed progress derived from LessonProgress.
        /// </summary>
        [HttpGet("my-subjects")]
        public async Task<IActionResult> GetMySubjects()
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
                return NotFound(new { message = "Student profile not found." });

            var studentId = student.student_id;

            // Load all enrollments + course + teacher in a single query
            var enrollments = await _db.Enrollments
                .AsNoTracking()
                .Include(e => e.Course)
                    .ThenInclude(c => c.Teacher)
                        .ThenInclude(t => t.User)
                .Include(e => e.Course)
                    .ThenInclude(c => c.CourseSessions)
                .Where(e => e.StudentId == studentId)
                .OrderByDescending(e => e.EnrolledAt)
                .ToListAsync();

            if (!enrollments.Any())
                return Ok(new List<SubjectListItemDto>());

            // Collect all courseIds to batch-fetch LessonProgress
            var courseIds = enrollments.Select(e => e.CourseId).ToList();

            // All session IDs across all enrolled courses
            var allSessionIds = enrollments
                .SelectMany(e => e.Course.CourseSessions.Select(s => s.Id))
                .ToList();

            // Load LessonProgress rows for this student, covering all enrolled courses
            var lessonProgressMap = await _db.LessonProgress
                .AsNoTracking()
                .Where(lp => lp.StudentId == studentId && allSessionIds.Contains(lp.CourseSessionId))
                .ToListAsync();

            // Build a lookup: sessionId → LessonProgress
            var progressBySession = lessonProgressMap
                .ToDictionary(lp => lp.CourseSessionId);

            var result = enrollments.Select(e =>
            {
                var sessions      = e.Course.CourseSessions.ToList();
                int totalSessions = sessions.Count;

                // A session is "completed" when the student has at least one view recorded
                int completedSessions = sessions.Count(s =>
                    progressBySession.TryGetValue(s.Id, out var lp) && lp.Views > 0);

                // Overall progress = completed / total × 100  (0 when no sessions)
                int progressPercent = totalSessions > 0
                    ? (int)Math.Round((double)completedSessions / totalSessions * 100)
                    : 0;

                var teacher = e.Course.Teacher;

                return new SubjectListItemDto
                {
                    CourseId          = e.CourseId,
                    Title             = e.Course.Title,
                    AcademicLevel     = e.Course.AcademicLevel,
                    AcademicYear      = e.Course.AcademicYear,
                    TeacherName       = teacher?.User != null
                                            ? $"{teacher.User.firstname} {teacher.User.lastname}".Trim()
                                            : "N/A",
                    TeacherSubject    = teacher?.subject ?? string.Empty,
                    ProgressPercent   = progressPercent,
                    TotalSessions     = totalSessions,
                    CompletedSessions = completedSessions,
                    EnrolledAt        = e.EnrolledAt
                };
            }).ToList();

            return Ok(result);
        }

        // ─────────────────────────────────────────────────────────────────
        // PAGE 4 – Course sessions detail
        // GET /api/subject/{courseId}/sessions
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Returns the full session list for a course the student is enrolled in.
        /// For each session the response includes:
        ///   – Watch progress (views used / remaining, percentage)
        ///   – Entry-test status (passed?, best score)
        ///   – Homework status (submitted?, grade)
        /// </summary>
        [HttpGet("{courseId:int}/sessions")]
        public async Task<IActionResult> GetCourseSessions(int courseId)
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
                return NotFound(new { message = "Student profile not found." });

            var studentId = student.student_id;

            // Verify enrollment – students may only view their own courses
            var enrolled = await _db.Enrollments
                .AsNoTracking()
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (!enrolled)
                return Forbid(); // 403 – not enrolled in this course

            // Load course with all sessions and entry tests
            var course = await _db.Courses
                .AsNoTracking()
                .Include(c => c.Teacher).ThenInclude(t => t.User)
                .Include(c => c.CourseSessions)
                    .ThenInclude(cs => cs.EntryTest)
                        .ThenInclude(q => q != null ? q.Questions : null)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
                return NotFound(new { message = "Course not found." });

            var sessions    = course.CourseSessions.OrderBy(s => s.Id).ToList();
            var sessionIds  = sessions.Select(s => s.Id).ToList();

            // ── Batch-load all student data for this course in three queries ──

            // 1. Lesson-level watch progress
            var lessonProgressList = await _db.LessonProgress
                .AsNoTracking()
                .Where(lp => lp.StudentId == studentId && sessionIds.Contains(lp.CourseSessionId))
                .ToListAsync();

            var progressBySession = lessonProgressList
                .ToDictionary(lp => lp.CourseSessionId);

            // 2. Homework submissions (one per session for this student)
            var homeworkSubmissions = await _db.HomeworkSubmissions
                .AsNoTracking()
                .Where(h => h.StudentId == studentId && sessionIds.Contains(h.SessionId))
                .ToListAsync();

            var homeworkBySession = homeworkSubmissions
                .ToDictionary(h => h.SessionId);

            // 3. Entry-test attempts – get best score per quiz
            //    We look at LessonAttempts which stores per-session quiz results
            var lessonAttempts = await _db.LessonAttempts
                .AsNoTracking()
                .Where(la => la.StudentId == studentId && sessionIds.Contains(la.CourseSessionId))
                .ToListAsync();

            // Group by session → best score and whether any attempt passed
            var attemptsBySession = lessonAttempts
                .GroupBy(la => la.CourseSessionId)
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        BestScore  = g.Max(a => (decimal)a.Score),
                        AnyPassed  = g.Any(a => a.Passed)
                    });

            // ── Compute overall course progress ──
            int totalSessions     = sessions.Count;
            int completedSessions = sessions.Count(s =>
                progressBySession.TryGetValue(s.Id, out var lp) && lp.Views > 0);
            int courseProgress    = totalSessions > 0
                ? (int)Math.Round((double)completedSessions / totalSessions * 100)
                : 0;

            // ── Build per-session DTOs ──
            var sessionDtos = sessions.Select(s =>
            {
                progressBySession.TryGetValue(s.Id, out var lp);
                homeworkBySession.TryGetValue(s.Id, out var hw);
                attemptsBySession.TryGetValue(s.Id, out var attempt);

                int viewsUsed      = lp?.Views ?? 0;
                int viewsRemaining = Math.Max(0, s.MaxViews - viewsUsed);

                bool hasEntryTest    = s.HasEntryTest && s.EntryTest != null;
                bool? entryTestPassed = hasEntryTest ? attempt?.AnyPassed : null;

                // Normalize best-score to 0-100 percentage if needed.
                // LessonAttempt.Score stores raw correct-answer count;
                // we convert using the quiz's question count when available.
                decimal? entryTestBestScore = null;
                if (hasEntryTest && attempt != null && s.EntryTest != null)
                {
                    int questionCount = s.EntryTest.Questions?.Count ?? 0;
                    entryTestBestScore = questionCount > 0
                        ? Math.Round(attempt.BestScore / questionCount * 100, 1)
                        : attempt.BestScore;
                }

                return new SessionItemDto
                {
                    SessionId              = s.Id,
                    Title                  = s.Title,
                    AvailableDays          = s.AvailableDays,
                    MaxViews               = s.MaxViews,
                    ViewsUsed              = viewsUsed,
                    ViewsRemaining         = viewsRemaining,
                    IsWatched              = viewsUsed > 0,
                    WatchProgressPercent   = lp?.ProgressPercent ?? 0.0,
                    HasAttachment          = !string.IsNullOrEmpty(s.AttachmentUrl),
                    AttachmentUrl          = s.AttachmentUrl,
                    HasHomework            = !string.IsNullOrEmpty(s.HomeworkUrl),
                    HomeworkUrl            = s.HomeworkUrl,
                    HasEntryTest           = hasEntryTest,
                    EntryTestPassed        = entryTestPassed,
                    EntryTestBestScore     = entryTestBestScore,
                    HomeworkSubmitted      = hw != null,
                    HomeworkGrade          = hw?.Grade
                };
            }).ToList();

            // ── Assemble top-level response ──
            var teacher = course.Teacher;
            var response = new CourseSessionsResponseDto
            {
                CourseId          = course.Id,
                CourseTitle       = course.Title,
                AcademicLevel     = course.AcademicLevel,
                AcademicYear      = course.AcademicYear,
                TeacherName       = teacher?.User != null
                                        ? $"{teacher.User.firstname} {teacher.User.lastname}".Trim()
                                        : "N/A",
                TeacherSubject    = teacher?.subject ?? string.Empty,
                ProgressPercent   = courseProgress,
                TotalSessions     = totalSessions,
                CompletedSessions = completedSessions,
                Sessions          = sessionDtos
            };

            return Ok(response);
        }
    }
}
