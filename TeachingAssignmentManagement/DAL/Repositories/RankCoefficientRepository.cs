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

        public IEnumerable<RankCoefficientDTO> GetStandardProgram(int startYear, int endYear)
        {
            return (from u in context.academic_degree_rank
                    join l in context.rank_coefficient on u.id equals l.academic_degree_rank_id into ranks
                    from rank in ranks.DefaultIfEmpty()
                    where rank.start_year == startYear && rank.end_year == endYear && rank.type == 0
                    select new RankCoefficientDTO
                    {
                        AcademicDegreeRankId = rank.academic_degree_rank_id,
                        UnitPrice = rank.unit_price,
                        VietnameseCoefficient = rank.vietnamese_coefficient,
                        ForeignCoefficient = rank.foreign_coefficient
                    }).ToList();
        }
    }
}