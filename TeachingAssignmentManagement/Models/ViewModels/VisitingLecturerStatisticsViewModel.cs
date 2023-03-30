using System.Collections.Generic;

namespace TeachingAssignmentManagement.Models.ViewModels
{
    public class VisitingLecturerStatisticsViewModel
    {
        public int[] TermIds { get; set; }
        public List<VisitingLecturerStatisticsDTO> VisitingLecturerStatisticsDTOs { get; set; }
    }
}