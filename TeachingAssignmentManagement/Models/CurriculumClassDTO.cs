namespace TeachingAssignmentManagement.Models
{
    public class CurriculumClassDTO
    {
        public int Id { get; set; }
        public string CurriculumClassId { get; set; }
        public int Day2 { get; set; }
        public int StartLesson2 { get; set; }
        public string RoomId { get; set; }
        public int TermId { get; set; }
        public string MajorId { get; set; }
        public string LecturerId { get; set; }
        public string CurriculumId { get; set; }
        public curriculum Curriculum { get; internal set; }
    }
}