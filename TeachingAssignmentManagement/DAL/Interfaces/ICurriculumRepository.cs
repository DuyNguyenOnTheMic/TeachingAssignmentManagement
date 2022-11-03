using System;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public interface ICurriculumRepository : IDisposable
    {
        void InsertCurriculum(curriculum curriculum);
    }
}