using System.Collections.Generic;

namespace TeachingAssignmentManagement.Models
{
    public class TimetableViewModel
    {
        public readonly List<int> days = new List<int> { 2, 3, 4, 5, 6, 7, 8 };
        public readonly List<int> startLessons = new List<int> { 1, 4, 7, 10, 13 };
        public IEnumerable<LecturerDTO> LecturerDTOs { get; set; }
        public IEnumerable<ClassSectionDTO> ClassSectionDTOs { get; set; }
    }
}