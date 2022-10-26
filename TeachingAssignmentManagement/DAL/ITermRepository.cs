using System;
using System.Collections;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public interface ITermRepository : IDisposable
    {
        IEnumerable GetTerms();
        term GetTermByID(int termId);
        void InsertTerm(term term);
        void DeleteTerm(int termId);
        void UpdateTerm(term term);
        void Save();
    }
}