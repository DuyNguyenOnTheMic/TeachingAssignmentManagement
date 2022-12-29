using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public class UserRepository
    {
        private readonly CP25Team03Entities context;

        public UserRepository(CP25Team03Entities context)
        {
            this.context = context;
        }

        public IEnumerable GetUsers()
        {
            DbSet<lecturer> query_lecturer = context.lecturers;
            return context.AspNetUsers.Select(u => new
            {
                id = u.Id,
                email = u.Email,
                role = u.AspNetRoles.FirstOrDefault().Name,
                query_lecturer.FirstOrDefault(l => l.id == u.Id).staff_id,
                query_lecturer.FirstOrDefault(l => l.id == u.Id).full_name,
                query_lecturer.FirstOrDefault(l => l.id == u.Id).type
            }).ToList();
        }

        public IEnumerable<AspNetRole> GetRoles()
        {
            return context.AspNetRoles.OrderBy(r => r.Id);
        }

        public AspNetRole GetRoleByID(string id)
        {
            return context.AspNetRoles.Find(id);
        }

        public IEnumerable<lecturer> GetLecturers()
        {
            return context.lecturers.Where(l => l.staff_id != null && l.full_name != null).ToList();
        }

        public lecturer GetLecturerByID(string id)
        {
            return context.lecturers.Find(id);
        }

        public lecturer GetLecturerByStaffId(string id)
        {
            return context.lecturers.FirstOrDefault(l => l.staff_id == id);
        }

        public IEnumerable<AspNetUser> GetFacultyBoards()
        {
            return context.AspNetUsers.Where(u => u.AspNetRoles.FirstOrDefault().Name == "BCN khoa");
        }

        public void InsertLecturer(lecturer lecturer)
        {
            context.lecturers.Add(lecturer);
        }

        public void UpdateLecturer(lecturer lecturer)
        {
            context.Entry(lecturer).State = EntityState.Modified;
        }
    }
}