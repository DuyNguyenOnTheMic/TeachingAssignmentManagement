using System;
using System.Collections;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public interface IMajorRepository : IDisposable
    {
        IEnumerable GetMajors();
        major GetMajorByID(string majorId);
        void InsertMajor(major major);
        void DeleteMajor(string majorId);
        void UpdateMajor(major major);
        void Save();
    }
}