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
        lecturer GetLecturerByID(string lecturerId);
        AspNetRole GetRoleByID(string roleId);
        void InsertLecturer(lecturer lecturer);
        void Save();
    }
}