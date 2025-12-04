namespace RESTfull.Domain.Entities
{
    public class StudyProgram
    {
        public int Id { get; set; }

        public string Institution { get; set; } = string.Empty;
        public string Faculty { get; set; } = string.Empty;
        public string Direction { get; set; } = string.Empty;
        public string Speciality { get; set; } = string.Empty;

        public List<Student> Students { get; set; } = new();
        public List<StudentGroup> StudentGroups { get; set; } = new();
    }
}
