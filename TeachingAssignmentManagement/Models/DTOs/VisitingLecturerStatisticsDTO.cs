using System.Collections.Generic;

namespace TeachingAssignmentManagement.Models
{
    public class VisitingLecturerStatisticsDTO
    {
        public string StaffId { get; set; }
        public string FullName { get; set; }
        public IEnumerable<subject> Subjects { get; set; }
    }
}