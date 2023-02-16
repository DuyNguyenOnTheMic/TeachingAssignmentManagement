using System.Collections;
using System.Data.Entity;
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
            return context.academic_degree.Select(a => new
            {
                a.id,
                a.name,
                a.level
            }).ToList();
        }

        public academic_degree GetAcademicDegreeByID(string id)
        {
            return context.academic_degree.Find(id);
        }

        public void InsertAcademicDegree(academic_degree academicDegree)
        {
            context.academic_degree.Add(academicDegree);
        }

        public void DeleteAcademicDegree(string academicDegreeId)
        {
            academic_degree academicDegree = context.academic_degree.Find(academicDegreeId);
            context.academic_degree.Remove(academicDegree);
        }

        public void UpdateAcademicDegree(academic_degree academicDegree)
        {
            context.Entry(academicDegree).State = EntityState.Modified;
        }
    }
}