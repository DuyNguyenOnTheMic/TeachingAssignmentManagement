using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public class UserRepository : IUserRepository, IDisposable
    {
        private readonly CP25Team03Entities context;

        public UserRepository(CP25Team03Entities context)
        {
            this.context = context;
        }

        public IEnumerable GetUsers()
        {
            var query_lecturer = context.lecturers;
            return context.AspNetUsers.Select(u => new
            {
                id = u.Id,
                email = u.Email,
                role = u.AspNetRoles.FirstOrDefault().Name,
                query_lecturer.FirstOrDefault(l => l.id == u.Id).staff_id,
                query_lecturer.FirstOrDefault(l => l.id == u.Id).full_name
            }).ToList();
        }

        public IEnumerable<AspNetRole> GetRoles()
        {
            return context.AspNetRoles;
        }

        public AspNetRole GetRoleByID(string id)
        {
            return context.AspNetRoles.Find(id);
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}