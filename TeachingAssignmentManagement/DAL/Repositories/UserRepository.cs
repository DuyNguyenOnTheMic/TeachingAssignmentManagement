﻿using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TeachingAssignmentManagement.Helpers;
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
            return (from u in context.AspNetUsers
                    join l in context.lecturers on u.Id equals l.id into lecturers
                    from lecturer in lecturers.DefaultIfEmpty()
                    select new
                    {
                        id = u.Id,
                        email = u.Email,
                        role = u.AspNetRoles.FirstOrDefault().Name,
                        lecturer.staff_id,
                        lecturer.full_name,
                        lecturer.type,
                        lecturer.status,
                        lecturer.is_vietnamese
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

        public IEnumerable<LecturerDTO> GetLecturers()
        {
            return (from u in context.AspNetUsers
                    join l in context.lecturers on u.Id equals l.id into lecturers
                    from lecturer in lecturers.DefaultIfEmpty()
                    where u.AspNetRoles.FirstOrDefault().Name != "Chưa phân quyền" && lecturer.staff_id != null && lecturer.full_name != null && lecturer.status == true
                    select new LecturerDTO
                    {
                        Id = lecturer.id,
                        FullName = lecturer.full_name,
                        Type = lecturer.type
                    }).ToList();
        }

        public IEnumerable<lecturer> GetFacultyMembersInTerm(int termId, string majorId)
        {
            return (majorId != "-1"
                ? context.class_section.Where(c => c.term_id == termId && c.major_id == majorId && c.lecturer.type == MyConstants.FacultyMemberType)
                : context.class_section.Where(c => c.term_id == termId && c.lecturer.type == MyConstants.FacultyMemberType)).Select(c => c.lecturer).Distinct();
        }

        public IEnumerable<lecturer> GetFacultyMembersInYear(int startYear, int endYear, string majorId)
        {
            return (majorId != "-1"
                ? context.class_section.Where(c => c.term.start_year == startYear && c.term.end_year == endYear && c.major_id == majorId && c.lecturer.type == MyConstants.FacultyMemberType)
                : context.class_section.Where(c => c.term.start_year == startYear && c.term.end_year == endYear && c.lecturer.type == MyConstants.FacultyMemberType)).Select(c => c.lecturer).Distinct();
        }

        public IEnumerable<AspNetUser> GetFacultyBoards()
        {
            return context.AspNetUsers.Where(u => u.AspNetRoles.FirstOrDefault().Name == CustomRoles.FacultyBoard);
        }

        public lecturer GetLecturerByID(string id)
        {
            return context.lecturers.Find(id);
        }

        public lecturer GetLecturerByStaffId(string id)
        {
            return context.lecturers.FirstOrDefault(l => l.staff_id == id);
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