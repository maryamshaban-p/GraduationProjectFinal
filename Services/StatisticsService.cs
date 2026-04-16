using grad.Data;
using grad.DTOs;
using Microsoft.EntityFrameworkCore;

namespace grad.Services
{
    /// <summary>
    /// Derives student statistics automatically from the database.
    /// No manual moderator input is required.
    ///
    ///   Absence  →  COUNT rows in StudentAbsences  WHERE StudentId = @id
    ///   Tasks    →  COUNT rows in HomeworkSubmissions WHERE StudentId = @id
    ///   Quiz     →  COUNT rows in StudentQuizResults  WHERE StudentId = @id
    /// </summary>
    public class StatisticsService : IStatisticsService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<StatisticsService> _logger;

        public StatisticsService(AppDbContext db, ILogger<StatisticsService> logger)
        {
            _db     = db;
            _logger = logger;
        }

        // ─────────────────────────────────────────────────────────────────────
        // SINGLE STUDENT
        // ─────────────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public async Task<StudentStatisticsDto> GetStudentStatisticsAsync(Guid studentId)
        {
            _logger.LogInformation(
                "Computing live statistics for student {StudentId}", studentId);

            // Run all three counts in parallel to minimise latency.
            // 1. هاتي عدد الغيابات
            var absenceCount = await _db.HomeworkSubmissions
                .CountAsync(h => h.StudentId == studentId); // تأكدي من اسم الجدول والحقل

            // 2. هاتي عدد التاسكات
            var tasksCount = await _db.HomeworkSubmissions
                .CountAsync(h => h.StudentId == studentId);

            // 3. هاتي عدد الكويزات
            var quizCount = await _db.StudentQuizResults
                .CountAsync(r => r.StudentId == studentId);

            // 4. رجعي النتيجة مباشرة
            return new StudentStatisticsDto
            {
                StudentId = studentId,
                Absence = absenceCount,
                Tasks = tasksCount,
                Quiz = quizCount
            };
        }

        // ─────────────────────────────────────────────────────────────────────
        // ALL STUDENTS  (moderator roster view)
        // ─────────────────────────────────────────────────────────────────────

        /// <inheritdoc/>
        public async Task<IEnumerable<StudentStatisticsDto>> GetAllStudentsStatisticsAsync()
        {
            _logger.LogInformation("Computing live statistics for all students");

            // Fetch all student IDs first.
            var studentIds = await _db.Students
                                      .Select(s => s.student_id)
                                      .ToListAsync();

            // Single-query aggregation per table — much faster than N+1 loops.
            var absenceCounts = await _db.StudentAbsences
                .GroupBy(a => a.StudentId)
                .Select(g => new { StudentId = g.Key, Count = g.Count() })
                .ToListAsync();

            var taskCounts = await _db.HomeworkSubmissions
                .GroupBy(h => h.StudentId)
                .Select(g => new { StudentId = g.Key, Count = g.Count() })
                .ToListAsync();

            var quizCounts = await _db.StudentQuizResults
                .GroupBy(r => r.StudentId)
                .Select(g => new { StudentId = g.Key, Count = g.Count() })
                .ToListAsync();

            // Convert to dictionaries for O(1) look-up.
            var absenceMap = absenceCounts.ToDictionary(x => x.StudentId, x => x.Count);
            var taskMap    = taskCounts.ToDictionary(x => x.StudentId, x => x.Count);
            var quizMap    = quizCounts.ToDictionary(x => x.StudentId, x => x.Count);

            // Build result — missing keys default to 0.
            return studentIds.Select(id => new StudentStatisticsDto
            {
                StudentId = id,
                Absence   = absenceMap.GetValueOrDefault(id, 0),
                Tasks     = taskMap.GetValueOrDefault(id, 0),
                Quiz      = quizMap.GetValueOrDefault(id, 0)
            });
        }
    }
}
