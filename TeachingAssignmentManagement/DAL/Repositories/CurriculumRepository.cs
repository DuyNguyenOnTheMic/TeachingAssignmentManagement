using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public class CurriculumRepository
    {
        private readonly CP25Team03Entities context;

        public CurriculumRepository(CP25Team03Entities context)
        {
            this.context = context;
        }

        public void InsertCurriculum(curriculum curriculum)
        {
            context.curricula.Add(curriculum);
        }

        public curriculum GetCurriculumByID(string id)
        {
            return context.curricula.Find(id);
        }
    }
}