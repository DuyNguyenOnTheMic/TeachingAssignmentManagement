using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TeachingAssignmentManagement.Helpers;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public class ClassSectionRepository
    {
        private readonly CP25Team03Entities context;

        public ClassSectionRepository(CP25Team03Entities context)
        {
            this.context = context;
        }

        public IEnumerable<ClassSectionDTO> GetTimetable(int termId, string lecturerId)
        {
            return context.class_section.Where(c => c.term_id == termId && c.lecturer_id == lecturerId).Select(c => new ClassSectionDTO
            {
                ClassSectionId = c.class_section_id,
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

        public IEnumerable<ClassSectionDTO> GetClassInWeek(IEnumerable<ClassSectionDTO> query_classes, int week)
        {
            return query_classes.Where(c => Array.Exists(c.LearnWeek.Split(','), element => int.Parse(element) == week)).ToList();
        }

        public IEnumerable<ClassSectionDTO> GetAssignTimetable(int termId, string majorId)
        {
            return context.class_section.Where(c => c.term_id == termId && c.major_id == majorId).Select(c => new ClassSectionDTO
            {
                Id = c.id,
                ClassSectionId = c.class_section_id,
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

        public IEnumerable<ClassSectionDTO> GetTermAssignTimetable(int termId)
        {
            return context.class_section.Where(c => c.term_id == termId).Select(c => new ClassSectionDTO
            {
                Id = c.id,
                ClassSectionId = c.class_section_id,
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

        public IEnumerable<ClassSectionDTO> GetTimetableStatistics(int termId)
        {
            return context.class_section.Where(c => c.term_id == termId).Select(c => new ClassSectionDTO
            {
                ClassSectionId = c.class_section_id,
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

        public IEnumerable<class_section> GetClassesInLesson(IEnumerable<class_section> classSections, int lesson)
        {
            return classSections.Where(c => c.start_lesson_2 == lesson);
        }

        public IEnumerable<class_section> GetClassesInCampus(IEnumerable<class_section> classSections, int lesson, string room)
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
                ? classSections.Where(c => !c.room_id.Contains(campus) && c.room_id.Contains("CS") && (c.start_lesson == previousLesson || c.start_lesson == nextLesson))
                : Enumerable.Empty<class_section>();
        }

        public IEnumerable<class_section> GetClassesInDay(IEnumerable<class_section> classSections, int day)
        {
            return classSections.Where(c => c.day_2 == day);
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
                    subject_count = c.GroupBy(item => item.subject.id).Count(),
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
                    subject_count = c.GroupBy(item => item.subject.id).Count(),
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
                    subject_count = c.GroupBy(item => item.subject.id).Count(),
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
                    subject_count = c.GroupBy(item => item.subject.id).Count(),
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

        public IEnumerable GetTermSubjects(int termId, string majorId, string lecturerId)
        {
            IQueryable<class_section> query_classes = majorId != "-1"
                ? context.class_section.Where(c => c.term_id == termId && c.major_id == majorId && c.lecturer_id == lecturerId)
                : context.class_section.Where(c => c.term_id == termId && c.lecturer_id == lecturerId);
            return query_classes.GroupBy(c => c.subject_id).Select(c => new
            {
                id = c.FirstOrDefault().subject.subject_id,
                subject_name = c.FirstOrDefault().subject.name,
                subject_credits = c.FirstOrDefault().subject.credits,
                subject_major = c.FirstOrDefault().major.name,
                subject_hours = c.Sum(item => item.total_lesson),
                theory_count = c.Count(item => item.type == MyConstants.TheoreticalClassType),
                practice_count = c.Count(item => item.type == MyConstants.PracticeClassType)
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
                    subject_count = c.GroupBy(item => item.subject.id).Count(),
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
                    subject_count = c.GroupBy(item => item.subject.id).Count(),
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
                    subject_count = c.GroupBy(item => item.subject.id).Count(),
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
                    subject_count = c.GroupBy(item => item.subject.id).Count(),
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

        public IEnumerable GetYearSubjects(int startYear, int endYear, string majorId, string lecturerId)
        {
            IQueryable<class_section> query_classes = majorId != "-1"
                ? context.class_section.Where(c => c.term.start_year == startYear && c.term.end_year == endYear && c.major_id == majorId && c.lecturer_id == lecturerId)
                : context.class_section.Where(c => c.term.start_year == startYear && c.term.end_year == endYear && c.lecturer_id == lecturerId);
            return query_classes.GroupBy(c => c.subject_id).Select(c => new
            {
                id = c.FirstOrDefault().subject.subject_id,
                subject_name = c.FirstOrDefault().subject.name,
                subject_credits = c.FirstOrDefault().subject.credits,
                subject_major = c.FirstOrDefault().major.name,
                subject_hours = c.Sum(item => item.total_lesson),
                theory_count = c.Count(item => item.type == MyConstants.TheoreticalClassType),
                practice_count = c.Count(item => item.type == MyConstants.PracticeClassType)
            }).ToList();
        }

        public IEnumerable<class_section> GetPersonalClassesInTerm(int termId, string lecturerId)
        {
            return context.class_section.Where(c => c.term_id == termId && c.lecturer_id == lecturerId);
        }

        public IEnumerable<class_section> GetPersonalClassesInYear(int startYear, int endYear, string lecturerId)
        {
            return context.class_section.Where(c => c.term.start_year == startYear && c.term.end_year == endYear && c.lecturer_id == lecturerId);
        }

        public IEnumerable<class_section> GetPersonalClassesInTermOrderBySubject(int termId, string majorId, string lecturerId)
        {
            return (majorId != "-1"
                ? context.class_section.Where(c => c.term_id == termId && c.major_id == majorId && c.lecturer_id == lecturerId)
                : context.class_section.Where(c => c.term_id == termId && c.lecturer_id == lecturerId)).OrderBy(c => c.subject.subject_id);
        }

        public IEnumerable<VisitingLecturerStatisticsDTO> GetVisitingLecturerStatistics(int[] termIds)
        {
            return context.class_section.Where(c => termIds.Contains(c.term_id) && c.lecturer.type == MyConstants.VisitingLecturerType && c.lecturer.staff_id != null && c.lecturer.full_name != null && c.lecturer.status == true).GroupBy(c => c.lecturer_id).Select(c => new VisitingLecturerStatisticsDTO
            {
                Id = c.Key,
                StaffId = c.FirstOrDefault().lecturer.staff_id,
                FullName = c.FirstOrDefault().lecturer.full_name,
                Subjects = c.Select(item => item.subject)
            }).ToList();
        }

        public bool CheckClassesInTermMajor(int termId, string majorId)
        {
            return majorId != "-1"
                ? context.class_section.Any(c => c.term_id == termId && c.major_id == majorId)
                : context.class_section.Any(c => c.term_id == termId);
        }

        public class_section FindClassSection(IEnumerable<class_section> classSection, string classSectionId, int day2, int startLesson2, string roomId)
        {
            return classSection.FirstOrDefault(c => c.class_section_id == classSectionId && c.day_2 == day2 && c.start_lesson_2 == startLesson2 && c.room_id == roomId);
        }

        public void InsertClassSection(class_section classSection)
        {
            context.class_section.Add(classSection);
        }

        public void DeleteClass(int id)
        {
            class_section classSection = context.class_section.Find(id);
            context.class_section.Remove(classSection);
        }

        public void DeleteAllClasses(int term, string major)
        {
            context.class_section.RemoveRange(context.class_section.Where(c => c.term_id == term && c.major_id == major));
        }
    }
}