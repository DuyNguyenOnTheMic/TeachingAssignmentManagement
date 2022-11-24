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

        public IEnumerable<CurriculumClassDTO> GetTimetable(int termId, string majorId)
        {
            return context.curriculum_class.Where(c => c.term_id == termId && c.major_id == majorId).Select(c => new CurriculumClassDTO
            {
                Id = c.id,
                CurriculumClassId = c.curriculum_class_id,
                Type = c.type,
                Day2 = c.day_2,
                StartLesson2 = c.start_lesson_2,
                TermId = c.term_id,
                MajorId = majorId,
                LecturerId = c.lecturer_id,
                LecturerName = c.lecturer.full_name,
                CurriculumId = c.curriculum_id,
                RoomId = c.room_id,
                Curriculum = c.curriculum
            });
        }

        public IEnumerable GetExportData(int termId, string majorId)
        {
            return context.curriculum_class.Where(c => c.term_id == termId && c.major_id == majorId).Select(c => new
            {
                c.original_id,
                c.curriculum_id,
                c.curriculum_class_id,
                c.curriculum.name,
                c.curriculum.credits,
                c.type,
                c.student_class_id,
                c.minimum_student,
                c.total_lesson,
                c.room.room_2,
                c.day,
                c.start_lesson,
                c.lesson_number,
                c.lesson_time,
                c.room.id,
                c.lecturer_id,
                c.lecturer.full_name,
                room_type = c.room.type,
                c.room.capacity,
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
            });
        }

        public IEnumerable<curriculum_class> GetClassesInTerm(int termId, string lecturerId)
        {
            return context.curriculum_class.Where(c => c.term_id == termId && c.lecturer_id == lecturerId);
        }

        public IEnumerable<curriculum_class> GetClassesInLesson(IEnumerable<curriculum_class> curriculumClasses, int lesson)
        {
            return curriculumClasses.Where(c => c.start_lesson_2 == lesson);
        }

        public IEnumerable<curriculum_class> GetClassesInDay(IEnumerable<curriculum_class> curriculumClasses, int day)
        {
            return curriculumClasses.Where(c => c.day_2 == day);
        }

        public IEnumerable<curriculum_class> GetClassesByTermMajor(int termId, string majorId)
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

        public void DeleteClass(int id)
        {
            curriculum_class curriculumClass = context.curriculum_class.Find(id);
            context.curriculum_class.Remove(curriculumClass);
        }

        public void DeleteAllClasses(int term, string major)
        {
            context.curriculum_class.RemoveRange(context.curriculum_class.Where(c => c.term_id == term && c.major_id == major));
        }
    }
}