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
            return context.rank_coefficient.Where(r => r.start_year == startYear && r.end_year == endYear).ToList();
        }
    }
}