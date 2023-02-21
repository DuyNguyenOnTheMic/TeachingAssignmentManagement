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

        public coefficient GetCoefficientInYear(int startYear, int endYear)
        {
            return context.coefficients.SingleOrDefault(r => r.start_year == startYear && r.end_year == endYear);
        }

        public coefficient GetCoefficientByID(int id)
        {
            return context.coefficients.Find(id);
        }

        public bool CheckCoefficientExists(int startYear, int endYear)
        {
            return context.coefficients.Any(r => r.start_year == startYear && r.end_year == endYear);
        }

        public void InsertCoefficient(coefficient coefficient)
        {
            context.coefficients.Add(coefficient);
        }
    }
}