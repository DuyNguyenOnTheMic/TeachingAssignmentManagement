using System;
using System.Collections;
using System.Collections.Generic;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public interface IUserRepository : IDisposable
    {
        IEnumerable GetUsers();
        IEnumerable<AspNetRole> GetRoles();
        AspNetRole GetRoleByID(string roleId);
        lecturer GetLecturerByID(string lecturerId);
        IEnumerable<AspNetUser> GetFacultyBoards();
        void InsertLecturer(lecturer lecturer);
        void Save();
    }
}