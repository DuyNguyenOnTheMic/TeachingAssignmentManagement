﻿using System.Collections.Generic;

namespace TeachingAssignmentManagement.Models
{
    public class VisitingLecturerStatisticsDTO
    {
        public string Id { get; set; }
        public string StaffId { get; set; }
        public string FullName { get; set; }
        public int TermId { get; set; }
        public IEnumerable<string> Subjects { get; set; }
    }
}