using System;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public interface ICurriculumClassRepository : IDisposable
    {
        void InsertCurriculumClass(curriculum_class curriculum_Class);
    }
}