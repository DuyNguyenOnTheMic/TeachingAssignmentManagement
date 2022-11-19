﻿using System.Collections.Generic;
using System.Linq;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public class CurriculumClassRepository
    {
        private readonly CP25Team03Entities context;

        public CurriculumClassRepository(CP25Team03Entities context)
        {
            this.context = context;
        }

        public IEnumerable<CurriculumClassDTO> Blabla(int termId, string majorId)
        {
            return context.curriculum_class.Select(c => new CurriculumClassDTO
            {
                id = c.id,
                curriculum_class_id = c.curriculum_class_id,
                curriculum_id = c.curriculum_id,
                curriculum = c.curriculum,
                day_2 = c.day_2,
                start_lesson_2 = c.start_lesson_2,
                lecturer_id = c.lecturer_id,
                term_id = c.term_id,
                major_id = majorId
            }).Where(c => c.term_id == termId && c.major_id == majorId);
        }

        public IEnumerable<curriculum_class> GetClassesInTermMajor(int termId, string majorId)
        {
            return context.curriculum_class.Where(c => c.term_id == termId && c.major_id == majorId);
        }

        public curriculum_class GetClassByID(int id)
        {
            return context.curriculum_class.Find(id);
        }

        public curriculum_class FindCurriculumClass(IEnumerable<curriculum_class> curriculumClass, string curriculumClassId, int day2, string roomId)
        {
            return curriculumClass.FirstOrDefault(c => c.curriculum_class_id == curriculumClassId && c.day_2 == day2 && c.room_id == roomId);
        }

        public curriculum_class CheckTermMajor(int termId, string majorId)
        {
            return context.curriculum_class.FirstOrDefault(c => c.term_id == termId && c.major_id == majorId);
        }

        public void InsertCurriculumClass(curriculum_class curriculum_Class)
        {
            context.curriculum_class.Add(curriculum_Class);
        }

        public void DeleteAllClasses(int term, string major)
        {
            context.curriculum_class.RemoveRange(context.curriculum_class.Where(c => c.term_id == term && c.major_id == major));
        }
    }
}