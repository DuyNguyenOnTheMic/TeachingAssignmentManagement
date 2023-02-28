namespace TeachingAssignmentManagement.Models
{
    public class RemunerationDTO
    {
        public string StaffId { get; set; }
        public string FullName { get; set; }
        public string AcademicDegreeRankId { get; set; }
        public decimal Remuneration { get; set; }
        public bool Status { get; set; }
        public int? OriginalHours { get; set; }
        public decimal RemunerationHours { get; set; }
        public decimal SumLesson1 { get; set; }
        public decimal SumLesson4 { get; set; }
        public decimal SumLesson7 { get; set; }
        public decimal SumLesson10 { get; set; }
        public decimal SumLesson13 { get; set; }
    }
}