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

        public IEnumerable<unit_price> GetUnitPriceInYear(int startYear, int endYear)
        {
            return context.unit_price.Where(r => r.start_year == startYear && r.end_year == endYear);
        }

        public unit_price GetUnitPriceByID(int id)
        {
            return context.unit_price.Find(id);
        }

        public bool CheckUnitPriceExists(int type, int startYear, int endYear, string rankId)
        {
            return context.unit_price.Any(r => r.type == type && r.start_year == startYear && r.end_year == endYear && r.academic_degree_rank_id == rankId);
        }

        public IEnumerable<UnitPriceDTO> GetUnitPriceByProgram(IEnumerable<unit_price> query_unitPrice, int type)
        {
            return query_unitPrice.Where(r => r.type == type).Select(r => new UnitPriceDTO
            {
                Id = r.id,
                UnitPrice = r.unit_price1,
                AcademicDegreeRankId = r.academic_degree_rank_id
            }).ToList();
        }

        public void InsertUnitPrice(unit_price unitPrice)
        {
            context.unit_price.Add(unitPrice);
        }

        public void DeleteAllUnitPrice(int type, int startYear, int endYear)
        {
            context.unit_price.RemoveRange(context.unit_price.Where(r => r.type == type && r.start_year == startYear && r.end_year == endYear));
        }
    }
}