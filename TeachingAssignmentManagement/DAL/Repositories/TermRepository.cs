using System;
using System.Collections;
using System.Data.Entity;
using System.Linq;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public class TermRepository : ITermRepository, IDisposable
    {
        private readonly CP25Team03Entities context;

        public TermRepository(CP25Team03Entities context)
        {
            this.context = context;
        }

        public IEnumerable GetTerms()
        {
            return context.terms.Select(t => new
            {
                t.id,
                t.start_year,
                t.end_year,
                t.start_week,
                t.start_date
            }).ToList();
        }

        public term GetTermByID(int id)
        {
            return context.terms.Find(id);
        }

        public void InsertTerm(term term)
        {
            context.terms.Add(term);
        }

        public void DeleteTerm(int termId)
        {
            term term = context.terms.Find(termId);
            context.terms.Remove(term);
        }

        public void UpdateTerm(term term)
        {
            context.Entry(term).State = EntityState.Modified;
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