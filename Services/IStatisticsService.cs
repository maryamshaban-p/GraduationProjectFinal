using grad.DTOs;

namespace grad.Services
{
    public interface IStatisticsService
    {
        /// <summary>
        /// Computes live statistics for a single student by counting rows in the
        /// relevant tables.  Always returns a valid dto — never null.
        /// Returns 0 for every counter when the student has no records yet.
        /// </summary>
        Task<StudentStatisticsDto> GetStudentStatisticsAsync(Guid studentId);

        /// <summary>
        /// Computes live statistics for every student in the system.
        /// Useful for moderator dashboards that show a full roster.
        /// </summary>
        Task<IEnumerable<StudentStatisticsDto>> GetAllStudentsStatisticsAsync();
    }
}
