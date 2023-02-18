using System.Collections.Generic;
using System.Linq;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public class RankCoefficientRepository
    {
        private readonly CP25Team03Entities context;

        public RankCoefficientRepository(CP25Team03Entities context)
        {
            this.context = context;
        }

        public IEnumerable<rank_coefficient> GetRankCoefficients(int startYear, int endYear)
        {
            return context.rank_coefficient.Where(r => r.start_year == startYear && r.end_year == endYear);
        }

        public IEnumerable<RankCoefficientDTO> GetStandardProgram(IEnumerable<rank_coefficient> query_rankCoefficients)
        {
            return query_rankCoefficients.Where(r => r.type == 0).Select(r => new RankCoefficientDTO
            {
                Id = r.id,
                UnitPrice = r.unit_price,
                VietnameseCoefficient = r.vietnamese_coefficient,
                ForeignCoefficient = r.foreign_coefficient
            }).ToList();
        }
    }
}