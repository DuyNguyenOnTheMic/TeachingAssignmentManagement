using System.Collections.Generic;
using System.Linq;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public class CoefficientRepository
    {
        private readonly CP25Team03Entities context;

        public CoefficientRepository(CP25Team03Entities context)
        {
            this.context = context;
        }

        public IEnumerable<coefficient> GetRankCoefficients(int startYear, int endYear)
        {
            return context.coefficients.Where(r => r.start_year == startYear && r.end_year == endYear);
        }

        public IEnumerable<RankCoefficientDTO> GetPrograms(IEnumerable<coefficient> query_rankCoefficients, int type)
        {
            return query_rankCoefficients.Where(r => r.type == type).Select(r => new RankCoefficientDTO
            {
                Id = r.id,
                UnitPrice = r.unit_price,
                VietnameseCoefficient = r.vietnamese_coefficient,
                ForeignCoefficient = r.foreign_coefficient,
                AcademicDegreeRankId = r.academic_degree_rank_id
            }).ToList();
        }

        public coefficient GetRankCoefficientByID(int id)
        {
            return context.coefficients.Find(id);
        }

        public bool CheckRankCoefficientExists(int type, int startYear, int endYear, string rankId)
        {
            return context.coefficients.Any(r.start_year == startYear && r.end_year == endYear && r.academic_degree_rank_id == rankId);
        }

        public void InsertRankCoefficient(coefficient rankCoefficient)
        {
            context.coefficients.Add(rankCoefficient);
        }

        public void DeleteAllRankCoefficients(int type, int startYear, int endYear)
        {
            context.coefficients.RemoveRange(context.coefficients.Where(r.start_year == startYear && r.end_year == endYear));
        }
    }
}