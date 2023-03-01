namespace TeachingAssignmentManagement.Models
{
    public class SubjectDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public string Major { get; set; }
        public int? Hours { get; set; }
        public int TheoryCount { get; set; }
        public int PracticeCount { get; set; }
    }
}