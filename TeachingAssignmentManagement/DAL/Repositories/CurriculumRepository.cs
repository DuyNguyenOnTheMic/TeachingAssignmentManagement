using System;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public class CurriculumRepository : ICurriculumRepository, IDisposable
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

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}