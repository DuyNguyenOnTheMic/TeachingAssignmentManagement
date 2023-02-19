﻿using System.Collections.Generic;

namespace TeachingAssignmentManagement.Models
{
    public class RankViewModels
    {
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public IEnumerable<AcademicDegreeRankDTO> AcademicDegreeRankDTOs { get; set; }
        public IEnumerable<RankCoefficientDTO> StandardProgramDTOs { get; set; }
        public IEnumerable<RankCoefficientDTO> SpecialProgramDTOs { get; set; }
        public IEnumerable<RankCoefficientDTO> ForeignDTOs { get; set; }
    }
}