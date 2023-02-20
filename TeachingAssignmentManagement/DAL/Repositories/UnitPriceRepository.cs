using System.Collections.Generic;
using System.Linq;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public class UnitPriceRepository
    {
        private readonly CP25Team03Entities context;

        public UnitPriceRepository(CP25Team03Entities context)
        {
            this.context = context;
        }

        public IEnumerable<unit_price> GetUnitPrices(int startYear, int endYear)
        {
            return context.unit_price.Where(r => r.start_year == startYear && r.end_year == endYear);
        }

        public IEnumerable<UnitPriceDTO> GetPrograms(IEnumerable<unit_price> query_unitPrices, int type)
        {
            return query_unitPrices.Where(r => r.type == type).Select(r => new UnitPriceDTO
            {
                Id = r.id,
                UnitPrice = r.unit_price1,
                AcademicDegreeRankId = r.academic_degree_rank_id
            }).ToList();
        }
    }
}