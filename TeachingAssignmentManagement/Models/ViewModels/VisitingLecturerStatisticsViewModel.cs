using System.Collections.Generic;

namespace TeachingAssignmentManagement.Models
{
    public class VisitingLecturerStatisticsViewModel
    {
        public int[] TermIds { get; set; }
        public IEnumerable<VisitingLecturerStatisticsDTO> VisitingLecturerStatisticsDTOs { get; set; }
    }
}