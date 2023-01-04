﻿using System;
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

        public IEnumerable<CurriculumClassDTO> GetTimetable(int termId, string lecturerId)
        {
            return context.class_section.Where(c => c.term_id == termId && c.lecturer_id == lecturerId).Select(c => new CurriculumClassDTO
            {
                CurriculumClassId = c.class_section_id,
                Type = c.type,
                LessonTime = c.lesson_time,
                Day2 = c.day_2,
                StartLesson2 = c.start_lesson_2,
                LearnWeek = c.learn_week,
                StartWeek = c.start_week,
                EndWeek = c.end_week,
                SubjectId = c.subject_id,
                RoomId = c.room_id,
                Subject = c.subject
            });
        }

        public IEnumerable<CurriculumClassDTO> GetClassInWeek(IEnumerable<CurriculumClassDTO> query_classes, int week)
        {
            return query_classes.Where(c => Array.Exists(c.LearnWeek.Split(','), element => int.Parse(element) == week));
        }

        public IEnumerable<CurriculumClassDTO> GetAssignTimetable(int termId, string majorId)
        {
            return context.class_section.Where(c => c.term_id == termId && c.major_id == majorId).Select(c => new CurriculumClassDTO
            {
                Id = c.id,
                CurriculumClassId = c.class_section_id,
                Type = c.type,
                Day2 = c.day_2,
                StartLesson2 = c.start_lesson_2,
                StudentRegisteredNumber = c.student_registered_number,
                LastAssigned = c.lecturer1.full_name,
                LecturerId = c.lecturer_id,
                LecturerName = c.lecturer.full_name,
                SubjectId = c.subject_id,
                RoomId = c.room_id,
                Subject = c.subject
            });
        }

        public IEnumerable<CurriculumClassDTO> GetTermAssignTimetable(int termId)
        {
            return context.class_section.Where(c => c.term_id == termId).Select(c => new CurriculumClassDTO
            {
                Id = c.id,
                CurriculumClassId = c.class_section_id,
                Type = c.type,
                Day2 = c.day_2,
                StartLesson2 = c.start_lesson_2,
                StudentRegisteredNumber = c.student_registered_number,
                LastAssigned = c.lecturer1.full_name,
                LecturerId = c.lecturer_id,
                LecturerName = c.lecturer.full_name,
                SubjectId = c.subject_id,
                RoomId = c.room_id,
                MajorAbb = c.major.abbreviation,
                Subject = c.subject
            });
        }

        public IEnumerable<CurriculumClassDTO> GetTimetableStatistics(int termId)
        {
            return context.class_section.Where(c => c.term_id == termId).Select(c => new CurriculumClassDTO
            {
                CurriculumClassId = c.class_section_id,
                Type = c.type,
                Day2 = c.day_2,
                StartLesson2 = c.start_lesson_2,
                LearnWeek = c.learn_week,
                StartWeek = c.start_week,
                EndWeek = c.end_week,
                LecturerId = c.lecturer_id,
                LecturerName = c.lecturer.full_name,
                RoomId = c.room_id,
                MajorName = c.major.name,
                MajorAbb = c.major.abbreviation,
                Subject = c.subject
            });
        }

        public IEnumerable<class_section> GetExportData(int termId, string majorId)
        {
            return context.class_section.Where(c => c.term_id == termId && c.major_id == majorId);
        }

        public IEnumerable<class_section> GetClassesInTerm(int termId, string lecturerId)
        {
            return context.class_section.Where(c => c.term_id == termId && c.lecturer_id == lecturerId);
        }

        public IEnumerable<class_section> GetClassesInLesson(IEnumerable<class_section> curriculumClasses, int lesson)
        {
            return curriculumClasses.Where(c => c.start_lesson_2 == lesson);
        }

        public IEnumerable<class_section> GetClassesInCampus(IEnumerable<class_section> curriculumClasses, int lesson, string room)
        {
            int previousLesson = 0;
            int nextLesson = lesson + 3;
            if (lesson != 1 && lesson != 7)
            {
                if (lesson == 4)
                {
                    nextLesson = 0;
                }
                previousLesson = lesson - 3;
            }
            string campus = room.Split('.')[0];
            return campus.Contains("CS")
                ? curriculumClasses.Where(c => !c.room_id.Contains(campus) && c.room_id.Contains("CS") && (c.start_lesson == previousLesson || c.start_lesson == nextLesson))
                : Enumerable.Empty<class_section>();
        }

        public IEnumerable<class_section> GetClassesInDay(IEnumerable<class_section> curriculumClasses, int day)
        {
            return curriculumClasses.Where(c => c.day_2 == day);
        }

        public IEnumerable<class_section> GetClassesByTerm(int termId)
        {
            return context.class_section.Where(c => c.term_id == termId);
        }

        public class_section GetClassByID(int id)
        {
            return context.class_section.Find(id);
        }

        public IEnumerable GetTermStatistics(bool isLesson, int termId, string majorId, string lecturerType)
        {
            IQueryable<class_section> query_classes = majorId != "-1"
                ? context.class_section.Where(c => c.term_id == termId && c.major_id == majorId && c.lecturer.type == lecturerType)
                : context.class_section.Where(c => c.term_id == termId && c.lecturer.type == lecturerType);
            if (!isLesson)
            {
                return query_classes.GroupBy(c => c.lecturer_id).Select(c => new
                {
                    c.Key,
                    c.FirstOrDefault().lecturer.staff_id,
                    c.FirstOrDefault().lecturer.full_name,
                    curriculum_count = c.GroupBy(item => item.subject.id).Count(),
                    class_count = c.Count(),
                    sum = c.Sum(item => item.total_lesson),
                    lecturer_type = c.FirstOrDefault().lecturer.type
                }).OrderByDescending(c => c.sum).ToList();
            }
            else
            {
                return query_classes.GroupBy(c => c.lecturer_id).Select(c => new
                {
                    c.Key,
                    c.FirstOrDefault().lecturer.staff_id,
                    c.FirstOrDefault().lecturer.full_name,
                    curriculum_count = c.GroupBy(item => item.subject.id).Count(),
                    class_count = c.Count(),
                    sum = c.Sum(item => item.total_lesson),
                    sumLesson1 = c.Where(item => item.start_lesson_2 == 1).Sum(item => item.total_lesson),
                    sumLesson4 = c.Where(item => item.start_lesson_2 == 4).Sum(item => item.total_lesson),
                    sumLesson7 = c.Where(item => item.start_lesson_2 == 7).Sum(item => item.total_lesson),
                    sumLesson10 = c.Where(item => item.start_lesson_2 == 10).Sum(item => item.total_lesson),
                    sumLesson13 = c.Where(item => item.start_lesson_2 == 13).Sum(item => item.total_lesson),
                    lecturer_type = c.FirstOrDefault().lecturer.type
                }).OrderByDescending(c => c.sum).ToList();
            }
        }

        public IEnumerable GetTermStatisticsAll(bool isLesson, int termId, string majorId)
        {
            IQueryable<class_section> query_classes = majorId != "-1"
                ? context.class_section.Where(c => c.term_id == termId && c.major_id == majorId && c.lecturer.type != null)
                : context.class_section.Where(c => c.term_id == termId && c.lecturer.type != null);
            if (!isLesson)
            {
                return query_classes.GroupBy(c => c.lecturer_id).Select(c => new
                {
                    c.Key,
                    c.FirstOrDefault().lecturer.staff_id,
                    c.FirstOrDefault().lecturer.full_name,
                    lecturer_type = c.FirstOrDefault().lecturer.type,
                    curriculum_count = c.GroupBy(item => item.subject.id).Count(),
                    class_count = c.Count(),
                    sum = c.Sum(item => item.total_lesson)
                }).OrderByDescending(c => c.sum).ToList();
            }
            else
            {
                return query_classes.GroupBy(c => c.lecturer_id).Select(c => new
                {
                    c.Key,
                    c.FirstOrDefault().lecturer.staff_id,
                    c.FirstOrDefault().lecturer.full_name,
                    curriculum_count = c.GroupBy(item => item.subject.id).Count(),
                    class_count = c.Count(),
                    sum = c.Sum(item => item.total_lesson),
                    sumLesson1 = c.Where(item => item.start_lesson_2 == 1).Sum(item => item.total_lesson),
                    sumLesson4 = c.Where(item => item.start_lesson_2 == 4).Sum(item => item.total_lesson),
                    sumLesson7 = c.Where(item => item.start_lesson_2 == 7).Sum(item => item.total_lesson),
                    sumLesson10 = c.Where(item => item.start_lesson_2 == 10).Sum(item => item.total_lesson),
                    sumLesson13 = c.Where(item => item.start_lesson_2 == 13).Sum(item => item.total_lesson),
                    lecturer_type = c.FirstOrDefault().lecturer.type
                }).OrderByDescending(c => c.sum).ToList();
            }
        }

        public IEnumerable GetTermCurriculums(int termId, string majorId, string lecturerId)
        {
            IQueryable<class_section> query_classes = majorId != "-1"
                ? context.class_section.Where(c => c.term_id == termId && c.major_id == majorId && c.lecturer_id == lecturerId)
                : context.class_section.Where(c => c.term_id == termId && c.lecturer_id == lecturerId);
            return query_classes.GroupBy(c => c.subject_id).Select(c => new
            {
                id = c.Key,
                curriculum_name = c.FirstOrDefault().subject.name,
                curriculum_credits = c.FirstOrDefault().subject.credits,
                curriculum_major = c.FirstOrDefault().major.name,
                curriculum_hours = c.Sum(item => item.total_lesson),
                theory_count = c.Count(item => item.type == "Lý thuyết"),
                practice_count = c.Count(item => item.type == "Thực hành")
            }).ToList();
        }

        public IEnumerable GetYearStatistics(bool isLesson, int startYear, int endYear, string majorId, string lecturerType)
        {
            IQueryable<class_section> query_classes = majorId != "-1"
                ? context.class_section.Where(c => c.term.start_year == startYear && c.term.end_year == endYear && c.major_id == majorId && c.lecturer.type == lecturerType)
                : context.class_section.Where(c => c.term.start_year == startYear && c.term.end_year == endYear && c.lecturer.type == lecturerType);
            if (!isLesson)
            {
                return query_classes.GroupBy(c => c.lecturer_id).Select(c => new
                {
                    c.Key,
                    c.FirstOrDefault().lecturer.staff_id,
                    c.FirstOrDefault().lecturer.full_name,
                    curriculum_count = c.GroupBy(item => item.subject.id).Count(),
                    class_count = c.Count(),
                    sum = c.Sum(item => item.total_lesson),
                    lecturer_type = c.FirstOrDefault().lecturer.type
                }).OrderByDescending(c => c.sum).ToList();
            }
            else
            {
                return query_classes.GroupBy(c => c.lecturer_id).Select(c => new
                {
                    c.Key,
                    c.FirstOrDefault().lecturer.staff_id,
                    c.FirstOrDefault().lecturer.full_name,
                    curriculum_count = c.GroupBy(item => item.subject.id).Count(),
                    class_count = c.Count(),
                    sum = c.Sum(item => item.total_lesson),
                    sumLesson1 = c.Where(item => item.start_lesson_2 == 1).Sum(item => item.total_lesson),
                    sumLesson4 = c.Where(item => item.start_lesson_2 == 4).Sum(item => item.total_lesson),
                    sumLesson7 = c.Where(item => item.start_lesson_2 == 7).Sum(item => item.total_lesson),
                    sumLesson10 = c.Where(item => item.start_lesson_2 == 10).Sum(item => item.total_lesson),
                    sumLesson13 = c.Where(item => item.start_lesson_2 == 13).Sum(item => item.total_lesson),
                    lecturer_type = c.FirstOrDefault().lecturer.type
                }).OrderByDescending(c => c.sum).ToList();
            }
        }

        public IEnumerable GetYearStatisticsAll(bool isLesson, int startYear, int endYear, string majorId)
        {
            IQueryable<class_section> query_classes = majorId != "-1"
                ? context.class_section.Where(c => c.term.start_year == startYear && c.term.end_year == endYear && c.major_id == majorId && c.lecturer.type != null)
                : context.class_section.Where(c => c.term.start_year == startYear && c.term.end_year == endYear && c.lecturer.type != null);
            if (!isLesson)
            {
                return query_classes.GroupBy(c => c.lecturer_id).Select(c => new
                {
                    c.Key,
                    c.FirstOrDefault().lecturer.staff_id,
                    c.FirstOrDefault().lecturer.full_name,
                    lecturer_type = c.FirstOrDefault().lecturer.type,
                    curriculum_count = c.GroupBy(item => item.subject.id).Count(),
                    class_count = c.Count(),
                    sum = c.Sum(item => item.total_lesson)
                }).OrderByDescending(c => c.sum).ToList();
            }
            else
            {
                return query_classes.GroupBy(c => c.lecturer_id).Select(c => new
                {
                    c.Key,
                    c.FirstOrDefault().lecturer.staff_id,
                    c.FirstOrDefault().lecturer.full_name,
                    curriculum_count = c.GroupBy(item => item.subject.id).Count(),
                    class_count = c.Count(),
                    sum = c.Sum(item => item.total_lesson),
                    sumLesson1 = c.Where(item => item.start_lesson_2 == 1).Sum(item => item.total_lesson),
                    sumLesson4 = c.Where(item => item.start_lesson_2 == 4).Sum(item => item.total_lesson),
                    sumLesson7 = c.Where(item => item.start_lesson_2 == 7).Sum(item => item.total_lesson),
                    sumLesson10 = c.Where(item => item.start_lesson_2 == 10).Sum(item => item.total_lesson),
                    sumLesson13 = c.Where(item => item.start_lesson_2 == 13).Sum(item => item.total_lesson),
                    lecturer_type = c.FirstOrDefault().lecturer.type
                }).OrderByDescending(c => c.sum).ToList();
            }
        }

        public IEnumerable GetYearCurriculums(int startYear, int endYear, string majorId, string lecturerId)
        {
            IQueryable<class_section> query_classes = majorId != "-1"
                ? context.class_section.Where(c => c.term.start_year == startYear && c.term.end_year == endYear && c.major_id == majorId && c.lecturer_id == lecturerId)
                : context.class_section.Where(c => c.term.start_year == startYear && c.term.end_year == endYear && c.lecturer_id == lecturerId);
            return query_classes.GroupBy(c => c.subject_id).Select(c => new
            {
                id = c.Key,
                curriculum_name = c.FirstOrDefault().subject.name,
                curriculum_credits = c.FirstOrDefault().subject.credits,
                curriculum_major = c.FirstOrDefault().major.name,
                curriculum_hours = c.Sum(item => item.total_lesson),
                theory_count = c.Count(item => item.type == "Lý thuyết"),
                practice_count = c.Count(item => item.type == "Thực hành")
            }).ToList();
        }

        public class_section FindCurriculumClass(IEnumerable<class_section> curriculumClass, string curriculumClassId, int day2, string roomId)
        {
            return curriculumClass.FirstOrDefault(c => c.class_section_id == curriculumClassId && c.day_2 == day2 && c.room_id == roomId);
        }

        public class_section CheckTermMajor(int termId, string majorId)
        {
            return context.class_section.FirstOrDefault(c => c.term_id == termId && c.major_id == majorId);
        }

        public void InsertCurriculumClass(class_section class_section)
        {
            context.class_section.Add(class_section);
        }

        public void DeleteClass(int id)
        {
            class_section curriculumClass = context.class_section.Find(id);
            context.class_section.Remove(curriculumClass);
        }

        public void DeleteAllClasses(int term, string major)
        {
            context.class_section.RemoveRange(context.class_section.Where(c => c.term_id == term && c.major_id == major));
        }
    }
}