using System.Collections;
using System.Collections.Generic;
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

        public IEnumerable GetCurriculumClasses()
        {
            return context.curriculum_class.Select(c => new
            {
                c.curriculum_class_id,
                c.original_id,
                c.student_class_id,
                c.minimum_student,
                c.total_lesson,
                c.day,
                c.start_lesson,
                c.lesson_number,
                c.lesson_time,
                c.student_number,
                c.free_slot,
                c.state,
                c.learn_week,
                c.day_2,
                c.start_lesson_2,
                c.student_registered_number,
                c.start_week,
                c.end_week,
                c.note_1,
                c.note_2
            }).ToList();
        }

        public void InsertCurriculumClass(curriculum_class curriculum_Class)
        {
            context.curriculum_class.Add(curriculum_Class);
        }

        public curriculum_class CheckTermMajor(int termId, string majorId)
        {
            return context.curriculum_class.FirstOrDefault(c => c.term_id == termId && c.major_id == majorId);
        }

        public IEnumerable<curriculum_class> GetClassesInTermMajor(int termId, string majorId)
        {
            return context.curriculum_class.Where(c => c.term_id == termId && c.major_id == majorId);
        }

        public curriculum_class FindCurriculumClass(IEnumerable<curriculum_class> curriculumClass, string curriculumClassId, int day2)
        {
            return curriculumClass.FirstOrDefault(c => c.curriculum_class_id == curriculumClassId && c.day_2 == day2);
        }

        public void DeleteAllClasses(int term, string major)
        {
            context.curriculum_class.RemoveRange(context.curriculum_class.Where(c => c.term_id == term && c.major_id == major));
        }
    }
}