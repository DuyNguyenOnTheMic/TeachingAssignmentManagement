using System.Collections.Generic;

namespace TeachingAssignmentManagement.Models
{
    public class TimetableViewModels
    {
        public readonly List<int> days = new List<int> { 2, 3, 4, 5, 6, 7 };
        public readonly List<int> startLessons = new List<int> { 1, 4, 7, 10, 13 };
        public IEnumerable<CurriculumClassDTO> CurriculumClassDTOs { get; set; }
    }
}