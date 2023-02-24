namespace TeachingAssignmentManagement.Models
{
    public class LecturerRankDTO
    {
        public int? Id { get; set; }
        public string LecturerId { get; set; }
        public string StaffId { get; set; }
        public string FullName { get; set; }
        public bool? IsVietnamese { get; set; }
        public string AcademicDegreeRankId { get; set; }
    }
}