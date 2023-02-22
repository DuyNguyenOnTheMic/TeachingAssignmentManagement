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
            return (from l in context.lecturers.Where(l => l.type == "TG")
                    join r in context.lecturer_rank.Where(r => r.term_id == term) on l.id equals r.lecturer_id into ranks
                    from rank in ranks.DefaultIfEmpty()
                    select new LecturerRankDTO
                    {
                        Id = rank.id,
                        LecturerId = l.id,
                        StaffId = l.staff_id,
                        FullName = l.full_name,
                        AcademicDegreeRankId = rank.academic_degree_rank_id
                    }).ToList();
        }

        public lecturer_rank GetLecturerRankByID(int id)
        {
            return context.lecturer_rank.Find(id);
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