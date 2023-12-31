﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public class SubjectRepository
    {
        private readonly CP25Team03Entities context;

        public SubjectRepository(CP25Team03Entities context)
        {
            this.context = context;
        }

        public IEnumerable<subject> GetSubjects(IEnumerable<ClassSectionDTO> classSections)
        {
            return classSections.Select(s => s.Subject).Distinct().ToList();
        }

        public IEnumerable GetSubjects(int termId, string majorId)
        {
            return context.subjects.Where(s => s.term_id == termId && s.major_id == majorId).Select(s => new
            {
                s.id,
                s.subject_id,
                s.name,
                s.credits,
                s.is_vietnamese
            }).ToList();
        }

        public IEnumerable GetTermSubjects(int termId)
        {
            return context.subjects.Where(s => s.term_id == termId).Select(s => new
            {
                s.id,
                s.subject_id,
                s.name,
                s.credits,
                major = s.major.name,
                s.is_vietnamese
            }).ToList();
        }

        public subject GetSubjectByID(string id)
        {
            return context.subjects.Find(id);
        }

        public void InsertSubject(subject subject)
        {
            context.subjects.Add(subject);
        }

        public void DeleteAllSubjects(int term, string major)
        {
            context.subjects.RemoveRange(context.subjects.Where(s => s.term_id == term && s.major_id == major));
        }
    }
}