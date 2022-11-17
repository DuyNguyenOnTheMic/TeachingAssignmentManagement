using System.Collections.Generic;
using System.Linq;
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

        public IEnumerable<curriculum> GetCurriculums()
        {
            return context.curriculum_class.Where(c => c.term_id == 222).Select(x => x.curriculum).Distinct().ToList();
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