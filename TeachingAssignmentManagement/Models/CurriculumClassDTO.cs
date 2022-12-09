namespace TeachingAssignmentManagement.Models
{
    public class CurriculumClassDTO
    {
        public int Id { get; set; }
        public string CurriculumClassId { get; set; }
        public string Type { get; set; }
        public string LessonTime { get; set; }
        public int Day2 { get; set; }
        public int StartLesson2 { get; set; }
        public int? StudentRegisteredNumber { get; set; }
        public int StartWeek { get; set; }
        public int EndWeek { get; set; }
        public string LecturerId { get; set; }
        public string LecturerName { get; set; }
        public string CurriculumId { get; set; }
        public string RoomId { get; set; }
        public string MajorAbb { get; set; }
        public curriculum Curriculum { get; set; }
    }
}