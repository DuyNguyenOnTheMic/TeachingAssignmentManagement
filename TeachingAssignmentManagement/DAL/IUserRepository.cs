using System;
using System.Collections;

namespace TeachingAssignmentManagement.DAL
{
    public interface IUserRepository : IDisposable
    {
        IEnumerable GetUsers();
    }
}