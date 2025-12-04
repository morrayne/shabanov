namespace RESTfull.Domain.Entities
{
    public class ContactInfo
    {
        public int Id { get; set; }

        // Use string for phone number for flexibility
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public int StudentId { get; set; }

        // Navigation back to student (no FK here - FK is on Student.ContactInfoId)
        public Student? Student { get; set; }
    }
}
