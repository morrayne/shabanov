using System;

namespace RESTfull.API.Models
{
    public class StudentCreateDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }

        // Use FK ids for existing StudyProgram and StudentGroup
        public int? StudyProgramId { get; set; }
        public int? StudentGroupId { get; set; }
    }
}