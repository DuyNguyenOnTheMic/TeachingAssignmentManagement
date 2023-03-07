namespace TeachingAssignmentManagement.Helpers
{
    public static class CustomRoles
    {
        public const string FacultyBoard = "BCN khoa";
        public const string Department = "Bộ môn";
        public const string Lecturer = "Giảng viên";
        public const string FacultyBoardOrDepartment = FacultyBoard + "," + Department;
        public const string AllRoles = FacultyBoardOrDepartment + "," + Lecturer;
    }
}