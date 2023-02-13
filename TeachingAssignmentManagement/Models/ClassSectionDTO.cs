namespace TeachingAssignmentManagement.Models
{
    public class ClassSectionDTO
    {
        public int Id { get; set; }
        public string ClassSectionId { get; set; }
        public string Type { get; set; }
        public string LessonTime { get; set; }
        public int Day2 { get; set; }
        public int StartLesson2 { get; set; }
        public int? StudentRegisteredNumber { get; set; }
        public string LearnWeek { get; set; }
        public int StartWeek { get; set; }
        public int EndWeek { get; set; }
        public string LastAssigned { get; set; }
        public string LecturerId { get; set; }
        public string LecturerName { get; set; }
        public string SubjectId { get; set; }
        public string RoomId { get; set; }
        public string MajorName { get; set; }
        public subject Subject { get; set; }
    }
}