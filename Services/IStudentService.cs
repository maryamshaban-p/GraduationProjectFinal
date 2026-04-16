using grad.DTOs;

namespace grad.Services
{
    public interface IStudentService
    {
        /// <summary>
        /// Creates a full student account: Identity user + Student record + teacher assignments.
        /// Returns the auto-generated credentials.
        /// </summary>
        Task<StudentCredentialsResponseDto> CreateStudentAsync(CreateStudentRequestDto dto);

        /// <summary>
        /// Replaces the teacher assignments for an existing student.
        /// </summary>
        Task AssignTeachersAsync(Guid studentId, List<Guid> teacherIds);

        /// <summary>
        /// Generates a unique username of the form firstname.lastname.year[_n].
        /// </summary>
        Task<string> GenerateUniqueUsernameAsync(string firstName, string lastName);

        /// <summary>
        /// Generates a random secure password (8+ chars, mixed letters + digits + symbol).
        /// </summary>
        string GenerateSecurePassword();
    }
}
