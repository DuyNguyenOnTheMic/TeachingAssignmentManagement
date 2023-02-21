using System.Collections.Generic;
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

        public IEnumerable<LecturerRankDTO> GetLecturerRanksInTerm(int term)
        {
            return (from u in context.lecturers.Where(l => l.type == "TG")
                    join l in context.lecturer_rank.Where(r => r.term_id == term) on u.id equals l.lecturer_id into lecturers
                    from lecturer in lecturers.DefaultIfEmpty()
                    select new LecturerRankDTO
                    {
                        Id = u.id,
                        StaffId = u.staff_id,
                        FullName = u.full_name,
                        AcademicDegreeRankId = lecturer.academic_degree_rank_id
                    }).ToList();
        }

        public void InsertLecturerRank(lecturer_rank lecturerRank)
        {
            context.lecturer_rank.Add(lecturerRank);
        }

        public void DeleteAllLecturerRanks(int term)
        {
            context.lecturer_rank.RemoveRange(context.lecturer_rank.Where(r => r.term_id == term));
        }
    }
}