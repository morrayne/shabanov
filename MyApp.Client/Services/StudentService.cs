using System.Net.Http.Json;
using RESTfull.Domain.Entities;
using MyApp.Client.Models;

namespace MyApp.Client.Services
{
    public class StudentService : IStudentService
    {
        private readonly HttpClient _http;

        public StudentService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Student>> GetAllAsync(string? search = null)
        {
            var url = "api/students";
            if (!string.IsNullOrWhiteSpace(search))
            {
                url += $"?search={System.Uri.EscapeDataString(search)}";
            }

            var res = await _http.GetFromJsonAsync<List<Student>>(url);
            return res ?? new List<Student>();
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<Student>($"api/students/{id}");
        }

        public async Task<(Student? student, string? error)> CreateAsync(StudentCreateDto dto)
        {
            var resp = await _http.PostAsJsonAsync("api/students", dto);
            if (resp.IsSuccessStatusCode)
            {
                var student = await resp.Content.ReadFromJsonAsync<Student>();
                return (student, null);
            }

            string? error = null;
            try
            {
                var body = await resp.Content.ReadFromJsonAsync<Dictionary<string, object?>>();
                if (body != null && body.TryGetValue("detail", out var d) && d != null)
                    error = d.ToString();
                else
                    error = resp.ReasonPhrase;
            }
            catch
            {
                error = resp.ReasonPhrase;
            }

            return (null, error);
        }

        public async Task<(bool ok, string? error)> UpdateAsync(int id, StudentCreateDto dto)
        {
            var resp = await _http.PutAsJsonAsync($"api/students/{id}", dto);
            if (resp.IsSuccessStatusCode) return (true, null);

            string? error = null;
            try
            {
                var body = await resp.Content.ReadFromJsonAsync<Dictionary<string, object?>>();
                if (body != null && body.TryGetValue("detail", out var d) && d != null)
                    error = d.ToString();
                else
                    error = resp.ReasonPhrase;
            }
            catch
            {
                error = resp.ReasonPhrase;
            }

            return (false, error);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var resp = await _http.DeleteAsync($"api/students/{id}");
            return resp.IsSuccessStatusCode;
        }

        public async Task<List<StudyProgram>> GetStudyProgramsAsync()
        {
            var res = await _http.GetFromJsonAsync<List<StudyProgram>>("api/studyprograms");
            return res ?? new List<StudyProgram>();
        }

        public async Task<List<StudentGroup>> GetGroupsAsync()
        {
            var res = await _http.GetFromJsonAsync<List<StudentGroup>>("api/studentgroups");
            return res ?? new List<StudentGroup>();
        }
    }
}
