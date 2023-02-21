﻿using System.Collections;
using System.Linq;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public class LecturerRankRepository
    {
        private readonly CP25Team03Entities context;

        public LecturerRankRepository(CP25Team03Entities context)
        {
            this.context = context;
        }

        public IEnumerable GetLecturerRanksInTerm(int term)
        {
            return (from u in context.lecturers.Where(l => l.type == "TG")
                    join l in context.lecturer_rank.Where(r => r.term_id == term) on u.id equals l.lecturer_id into lecturers
                    from lecturer in lecturers.DefaultIfEmpty()
                    select new
                    {
                        u.id,
                        u.staff_id,
                        u.full_name,
                        lecturer.academic_degree_rank_id
                    }).ToList();
        }
    }
}