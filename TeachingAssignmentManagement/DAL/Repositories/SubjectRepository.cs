using System.Collections.Generic;
using System.Linq;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public class SubjectRepository
    {
        private readonly CP25Team03Entities context;

        public SubjectRepository(CP25Team03Entities context)
        {
            this.context = context;
        }

        public IEnumerable<subject> GetCurriculums(IEnumerable<ClassSectionDTO> curriculumClass)
        {
            return curriculumClass.Select(c => c.Subject).Distinct().ToList();
        }

        public subject GetCurriculumByID(string id)
        {
            return context.subjects.Find(id);
        }

        public void InsertCurriculum(subject subject)
        {
            context.subjects.Add(subject);
        }
    }
}