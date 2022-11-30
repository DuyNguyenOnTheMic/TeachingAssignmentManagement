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

        public IEnumerable<CurriculumClassDTO> GetTimetable(int termId, string lecturerId)
        {
            return context.curriculum_class.Where(c => c.term_id == termId && c.lecturer_id == lecturerId).Select(c => new CurriculumClassDTO
            {
                CurriculumClassId = c.curriculum_class_id,
                Type = c.type,
                LessonTime = c.lesson_time,
                Day2 = c.day_2,
                StartLesson2 = c.start_lesson_2,
                StartWeek = c.start_week,
                EndWeek = c.end_week,
                CurriculumId = c.curriculum_id,
                RoomId = c.room_id,
                Curriculum = c.curriculum
            });
        }

        public IEnumerable<CurriculumClassDTO> GetClassInWeek(IEnumerable<CurriculumClassDTO> query_classes, int week)
        {
            return query_classes.Where(c => c.StartWeek <= week && c.EndWeek >= week);
        }

        public IEnumerable<CurriculumClassDTO> GetAssignTimetable(int termId, string majorId)
        {
            return context.curriculum_class.Where(c => c.term_id == termId && c.major_id == majorId).Select(c => new CurriculumClassDTO
            {
                Id = c.id,
                CurriculumClassId = c.curriculum_class_id,
                Type = c.type,
                Day2 = c.day_2,
                StartLesson2 = c.start_lesson_2,
                LecturerId = c.lecturer_id,
                LecturerName = c.lecturer.full_name,
                CurriculumId = c.curriculum_id,
                RoomId = c.room_id,
                Curriculum = c.curriculum
            });
        }

        public IEnumerable<curriculum_class> GetExportData(int termId, string majorId)
        {
            return context.curriculum_class.Where(c => c.term_id == termId && c.major_id == majorId);
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