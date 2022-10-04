using System;
using System.Collections.Generic;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public interface IMajorRepository : IDisposable
    {
        IEnumerable<major> GetMajors(bool final);
        major GetMajorByID(string majorId);
        void InsertMajor(major major);
        void DeleteMajor(string majorId);
        void UpdateMajor(major major);
        void Save();
    }
}