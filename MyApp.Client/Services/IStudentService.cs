using RESTfull.Domain.Entities;
using MyApp.Client.Models;

namespace MyApp.Client.Services
{
    public interface IStudentService
    {
        Task<List<Student>> GetAllAsync(string? search = null);
        Task<Student?> GetByIdAsync(int id);
        Task<(Student? student, string? error)> CreateAsync(StudentCreateDto dto);
        Task<(bool ok, string? error)> UpdateAsync(int id, StudentCreateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<List<StudyProgram>> GetStudyProgramsAsync();
        Task<List<StudentGroup>> GetGroupsAsync();
    }
}
