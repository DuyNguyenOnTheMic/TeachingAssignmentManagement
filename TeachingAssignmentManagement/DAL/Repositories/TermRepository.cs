using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public class TermRepository
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
                t.start_date,
                t.max_lesson,
                t.max_class
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
    }
}