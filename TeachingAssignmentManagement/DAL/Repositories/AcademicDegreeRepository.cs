using System.Collections;
using System.Linq;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public class AcademicDegreeRepository
    {
        private readonly CP25Team03Entities context;

        public AcademicDegreeRepository(CP25Team03Entities context)
        {
            this.context = context;
        }

        public IEnumerable GetAcademicDegrees()
        {
            return context.academic_degree_rank.Select(a => new
            {
                a.id,
                a.academic_degree_id,
                a.academic_degree.name
            }).ToList();
        }
    }
}