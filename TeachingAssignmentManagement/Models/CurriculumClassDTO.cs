namespace TeachingAssignmentManagement.Models
{
    public class CurriculumClassDTO
    {
        public int id { get; set; }
        public string curriculum_class_id { get; set; }
        public int day_2 { get; set; }
        public int start_lesson_2 { get; set; }
        public string room_id { get; set; }
        public int term_id { get; set; }
        public string major_id { get; set; }
        public string lecturer_id { get; set; }
        public string curriculum_id { get; set; }
        public curriculum curriculum { get; internal set; }
    }
}