using System;
using System.Collections;
using System.Data.Entity;
using System.Linq;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public class MajorRepository : IMajorRepository, IDisposable
    {
        private readonly CP25Team03Entities context;

        public MajorRepository(CP25Team03Entities context)
        {
            this.context = context;
        }

        public IEnumerable GetMajors()
        {
            return context.majors.Select(m => new
            {
                m.id,
                m.name
            }).ToList();
        }

        public major GetMajorByID(string id)
        {
            return context.majors.Find(id);
        }

        public void InsertMajor(major major)
        {
            context.majors.Add(major);
        }

        public void DeleteMajor(string majorId)
        {
            major major = context.majors.Find(majorId);
            context.majors.Remove(major);
        }

        public void UpdateMajor(major major)
        {
            context.Entry(major).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
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