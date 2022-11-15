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
            IQueryable<curriculum_class> query_curriculumClass_mon = context.curriculum_class.Where(x => x.day_2 == 2);
            IQueryable<curriculum_class> query_curriculumClass_tues = context.curriculum_class.Where(x => x.day_2 == 3);
            IQueryable<curriculum_class> query_curriculumClass_wed = context.curriculum_class.Where(x => x.day_2 == 4);
            IQueryable<curriculum_class> query_curriculumClass_thurs = context.curriculum_class.Where(x => x.day_2 == 5);
            IQueryable<curriculum_class> query_curriculumClass_fri = context.curriculum_class.Where(x => x.day_2 == 6);
            IQueryable<curriculum_class> query_curriculumClass_sat = context.curriculum_class.Where(x => x.day_2 == 7);
            return context.curricula.Select(c => new
            {
                c.id,
                c.name,
                mon1 = query_curriculumClass_mon.FirstOrDefault(m => m.start_lesson_2 == 1 && m.curriculum_id == c.id).curriculum_class_id,
                mon4 = query_curriculumClass_mon.FirstOrDefault(m => m.start_lesson_2 == 4 && m.curriculum_id == c.id).curriculum_class_id,
                mon7 = query_curriculumClass_mon.FirstOrDefault(m => m.start_lesson_2 == 7 && m.curriculum_id == c.id).curriculum_class_id,
                mon10 = query_curriculumClass_mon.FirstOrDefault(m => m.start_lesson_2 == 10 && m.curriculum_id == c.id).curriculum_class_id,
                mon13 = query_curriculumClass_mon.FirstOrDefault(m => m.start_lesson_2 == 13 && m.curriculum_id == c.id).curriculum_class_id,
                tues1 = query_curriculumClass_tues.FirstOrDefault(m => m.start_lesson_2 == 1 && m.curriculum_id == c.id).curriculum_class_id,
                tues4 = query_curriculumClass_tues.FirstOrDefault(m => m.start_lesson_2 == 4 && m.curriculum_id == c.id).curriculum_class_id,
                tues7 = query_curriculumClass_tues.FirstOrDefault(m => m.start_lesson_2 == 7 && m.curriculum_id == c.id).curriculum_class_id,
                tues10 = query_curriculumClass_tues.FirstOrDefault(m => m.start_lesson_2 == 10 && m.curriculum_id == c.id).curriculum_class_id,
                tues13 = query_curriculumClass_tues.FirstOrDefault(m => m.start_lesson_2 == 13 && m.curriculum_id == c.id).curriculum_class_id,
                wed1 = query_curriculumClass_wed.FirstOrDefault(m => m.start_lesson_2 == 1 && m.curriculum_id == c.id).curriculum_class_id,
                wed4 = query_curriculumClass_wed.FirstOrDefault(m => m.start_lesson_2 == 4 && m.curriculum_id == c.id).curriculum_class_id,
                wed7 = query_curriculumClass_wed.FirstOrDefault(m => m.start_lesson_2 == 7 && m.curriculum_id == c.id).curriculum_class_id,
                wed10 = query_curriculumClass_wed.FirstOrDefault(m => m.start_lesson_2 == 10 && m.curriculum_id == c.id).curriculum_class_id,
                wed13 = query_curriculumClass_wed.FirstOrDefault(m => m.start_lesson_2 == 13 && m.curriculum_id == c.id).curriculum_class_id,
                thurs1 = query_curriculumClass_thurs.FirstOrDefault(m => m.start_lesson_2 == 1 && m.curriculum_id == c.id).curriculum_class_id,
                thurs4 = query_curriculumClass_thurs.FirstOrDefault(m => m.start_lesson_2 == 4 && m.curriculum_id == c.id).curriculum_class_id,
                thurs7 = query_curriculumClass_thurs.FirstOrDefault(m => m.start_lesson_2 == 7 && m.curriculum_id == c.id).curriculum_class_id,
                thurs10 = query_curriculumClass_thurs.FirstOrDefault(m => m.start_lesson_2 == 10 && m.curriculum_id == c.id).curriculum_class_id,
                thurs13 = query_curriculumClass_thurs.FirstOrDefault(m => m.start_lesson_2 == 13 && m.curriculum_id == c.id).curriculum_class_id,
                fri1 = query_curriculumClass_fri.FirstOrDefault(m => m.start_lesson_2 == 1 && m.curriculum_id == c.id).curriculum_class_id,
                fri4 = query_curriculumClass_fri.FirstOrDefault(m => m.start_lesson_2 == 4 && m.curriculum_id == c.id).curriculum_class_id,
                fri7 = query_curriculumClass_fri.FirstOrDefault(m => m.start_lesson_2 == 7 && m.curriculum_id == c.id).curriculum_class_id,
                fri10 = query_curriculumClass_fri.FirstOrDefault(m => m.start_lesson_2 == 10 && m.curriculum_id == c.id).curriculum_class_id,
                fri13 = query_curriculumClass_fri.FirstOrDefault(m => m.start_lesson_2 == 13 && m.curriculum_id == c.id).curriculum_class_id,
                sat1 = query_curriculumClass_sat.FirstOrDefault(m => m.start_lesson_2 == 1 && m.curriculum_id == c.id).curriculum_class_id,
                sat4 = query_curriculumClass_sat.FirstOrDefault(m => m.start_lesson_2 == 4 && m.curriculum_id == c.id).curriculum_class_id,
                sat7 = query_curriculumClass_sat.FirstOrDefault(m => m.start_lesson_2 == 7 && m.curriculum_id == c.id).curriculum_class_id,
                sat10 = query_curriculumClass_sat.FirstOrDefault(m => m.start_lesson_2 == 10 && m.curriculum_id == c.id).curriculum_class_id,
                sat13 = query_curriculumClass_sat.FirstOrDefault(m => m.start_lesson_2 == 13 && m.curriculum_id == c.id).curriculum_class_id,
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