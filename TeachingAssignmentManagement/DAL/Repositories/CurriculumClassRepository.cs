using System.Data.Entity;
using System.Linq;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public class CurriculumClassRepository
    {
        private readonly CP25Team03Entities context;

        public CurriculumClassRepository(CP25Team03Entities context)
        {
            this.context = context;
        }

        public void InsertCurriculumClass(curriculum_class curriculum_Class)
        {
            context.curriculum_class.Add(curriculum_Class);
        }

        public curriculum_class GetCurriculumClassByID(string id)
        {
            return context.curriculum_class.Find(id);
        }

        public curriculum_class CheckTermMajor(int termId, string majorId)
        {
            return context.curriculum_class.FirstOrDefault(c => c.term_id == termId && c.major_id == majorId);
        }

        public void UpdateCurriculumClass(curriculum_class curriculumClass)
        {
            context.Entry(curriculumClass).State = EntityState.Modified;
        }
    }
}