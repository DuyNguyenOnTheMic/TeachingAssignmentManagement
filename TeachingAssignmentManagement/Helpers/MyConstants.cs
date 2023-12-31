﻿using System.Collections.Generic;

namespace TeachingAssignmentManagement.Helpers
{
    public static class MyConstants
    {
        // Timetable constants
        public static readonly string TheoreticalClassType = "Lý thuyết";
        public static readonly string PracticeClassType = "Thực hành";

        // Lecturer type constants
        public static readonly string FacultyMemberType = "CH";
        public static readonly string VisitingLecturerType = "TG";

        // Remuneration constants
        public static readonly int StandardProgramType = 0;
        public static readonly int SpecialProgramType = 1;
        public static readonly int ForeignType = 2;
        public static readonly Dictionary<int, string> ProgramTypes = new Dictionary<int, string> {
            {StandardProgramType, "CTĐT tiêu chuẩn"},
            {SpecialProgramType, "CTĐT đặc biệt" },
            {ForeignType, "Người nước ngoài" }
        };
    }
}