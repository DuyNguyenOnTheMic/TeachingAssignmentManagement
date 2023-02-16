using System.Collections;
using System.Data.Entity;
using System.Linq;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public class AcademicDegreeRankRepository
    {
        private readonly CP25Team03Entities context;

        public AcademicDegreeRankRepository(CP25Team03Entities context)
        {
            this.context = context;
        }

        public IEnumerable GetAcademicDegreeRanks()
        {
            return context.academic_degree_rank.Select(a => new
            {
                a.id,
                group = a.academic_degree_id + " " + a.academic_degree.name,
            }).ToList();
        }

        public academic_degree_rank GetAcademicDegreeRankByID(string id)
        {
            return context.academic_degree_rank.Find(id);
        }

        public void InsertAcademicDegreeRank(academic_degree_rank academicDegreeRank)
        {
            context.academic_degree_rank.Add(academicDegreeRank);
        }

        public void DeleteAcademicDegreeRank(string academicDegreeRankId)
        {
            academic_degree_rank academicDegreeRank = context.academic_degree_rank.Find(academicDegreeRankId);
            context.academic_degree_rank.Remove(academicDegreeRank);
        }

        public void UpdateAcademicDegreeRank(academic_degree_rank academicDegreeRank)
        {
            context.Entry(academicDegreeRank).State = EntityState.Modified;
        }
    }
}