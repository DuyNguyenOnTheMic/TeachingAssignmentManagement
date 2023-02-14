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

        public IEnumerable<subject> GetSubjects(IEnumerable<ClassSectionDTO> classSections)
        {
            return classSections.Select(c => c.Subject).Distinct().ToList();
        }

        public IEnumerable<subject> GetSubjects(int termId, string majorId)
        {
            return context.subjects.Where(c => c.term_id == termId && c.major_id == majorId).ToList();
        }

        public IEnumerable<subject> GetTermSubjects(int termId)
        {
            return context.subjects.Where(c => c.term_id == termId).ToList();
        }

        public subject GetSubjectByID(string id)
        {
            return context.subjects.Find(id);
        }

        public void InsertSubject(subject subject)
        {
            context.subjects.Add(subject);
        }

        public void DeleteAllSubjects(int term, string major)
        {
            context.subjects.RemoveRange(context.subjects.Where(c => c.term_id == term && c.major_id == major));
        }
    }
}