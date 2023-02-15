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
    }
}