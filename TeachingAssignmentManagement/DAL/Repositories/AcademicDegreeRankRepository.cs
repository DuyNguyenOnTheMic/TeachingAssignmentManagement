using System.Collections;
using System.Collections.Generic;
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
                group = a.academic_degree.level + ". " + a.academic_degree_id + " (" + a.academic_degree.name + ")"
            }).ToList();
        }

        public IEnumerable<academic_degree_rank> GetAcademicDegreeRankDTO()
        {
            return context.academic_degree_rank.ToList();
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