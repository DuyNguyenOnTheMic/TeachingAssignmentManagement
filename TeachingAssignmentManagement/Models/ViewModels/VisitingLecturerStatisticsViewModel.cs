using System.Collections.Generic;

namespace TeachingAssignmentManagement.Models.ViewModels
{
    public class VisitingLecturerStatisticsViewModel
    {
        public int[] TermList { get; set; }
        public List<VisitingLecturerStatisticsDTO> lecturerDTOs { get; set; }
    }
}