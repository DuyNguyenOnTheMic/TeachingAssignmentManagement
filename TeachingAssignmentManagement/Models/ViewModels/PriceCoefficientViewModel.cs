using System.Collections.Generic;

namespace TeachingAssignmentManagement.Models
{
    public class PriceCoefficientViewModel
    {
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public int ProgramType { get; set; }
        public IEnumerable<AcademicDegreeRankDTO> AcademicDegreeRankDTOs { get; set; }
        public IEnumerable<UnitPriceDTO> ProgramDTOs { get; set; }
        public coefficient Coefficients { get; set; }
    }
}