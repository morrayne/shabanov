namespace RESTfull.Domain.Entities
{
    public class Student
    {
        public int Id { get; set; }

        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        // Foreign key properties for program/group remain nullable
        public int? StudyProgramId { get; set; }
        public int? StudentGroupId { get; set; }

        // Navigation properties
        public StudyProgram? StudyProgram { get; set; }
        public StudentGroup? StudentGroup { get; set; }
    }
}
