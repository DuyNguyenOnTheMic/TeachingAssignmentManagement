using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public class LecturerRankRepository
    {
        private readonly CP25Team03Entities context;

        public LecturerRankRepository(CP25Team03Entities context)
        {
            this.context = context;
        }
    }
}