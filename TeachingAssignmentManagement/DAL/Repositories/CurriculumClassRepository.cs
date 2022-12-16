using DocumentFormat.OpenXml.Office.CustomUI;
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
                StudentRegisteredNumber = c.student_registered_number,
                LecturerId = c.lecturer_id,
                LecturerName = c.lecturer.full_name,
                CurriculumId = c.curriculum_id,
                RoomId = c.room_id,
                Curriculum = c.curriculum
            });
        }

        public IEnumerable<CurriculumClassDTO> GetTermAssignTimetable(int termId)
        {
            return context.curriculum_class.Where(c => c.term_id == termId).Select(c => new CurriculumClassDTO
            {
                Id = c.id,
                CurriculumClassId = c.curriculum_class_id,
                Type = c.type,
                Day2 = c.day_2,
                StartLesson2 = c.start_lesson_2,
                StudentRegisteredNumber = c.student_registered_number,
                LecturerId = c.lecturer_id,
                LecturerName = c.lecturer.full_name,
                CurriculumId = c.curriculum_id,
                RoomId = c.room_id,
                MajorAbb = c.major.abbreviation,
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

        public IEnumerable<curriculum_class> GetClassesInCampus(IEnumerable<curriculum_class> curriculumClasses, int lesson, string room)
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
                : Enumerable.Empty<curriculum_class>();
        }

        public IEnumerable<curriculum_class> GetClassesInDay(IEnumerable<curriculum_class> curriculumClasses, int day)
        {
            return curriculumClasses.Where(c => c.day_2 == day);
        }

        public IEnumerable<curriculum_class> GetClassesByTerm(int termId)
        {
            return context.curriculum_class.Where(c => c.term_id == termId);
        }

        public curriculum_class GetClassByID(int id)
        {
            return context.curriculum_class.Find(id);
        }

        public IEnumerable GetTermStatistics(int termId, string lecturerType)
        {
            return context.curriculum_class.Where(c => c.term_id == termId && c.lecturer.type == lecturerType).GroupBy(c => c.lecturer_id).Select(c => new
            {
                c.FirstOrDefault().lecturer.staff_id,
                c.FirstOrDefault().lecturer.full_name,
                sum = c.Sum(item => item.total_lesson),
                theory_count = c.GroupBy(item => item.curriculum.id).Select(item => item.Count(cu => cu.type == "Lý thuyết")),
                practice_count = c.GroupBy(item => item.curriculum.id).Select(item => item.Count(cu => cu.type == "Thực hành")),
                curriculum_count = c.GroupBy(item => item.curriculum.id).Count(),
                curriculum_id = c.GroupBy(item => item.curriculum.id).Select(item => item.FirstOrDefault().curriculum_id),
                curriculum_name = c.GroupBy(item => item.curriculum.id).Select(item => item.FirstOrDefault().curriculum.name),
                curriculum_credits = c.GroupBy(item => item.curriculum.id).Select(item => item.FirstOrDefault().curriculum.credits),
                curriculum_major = c.GroupBy(item => item.curriculum.id).Select(item => item.FirstOrDefault().major.name),
                curriculum_hours = c.GroupBy(item => item.curriculum.id).Select(item => item.Sum(cu => cu.total_lesson))
            }).OrderByDescending(c => c.sum).ToList();
        }

        public IEnumerable GetTermStatisticsAll(int termId)
        {
            return context.curriculum_class.Where(c => c.term_id == termId && c.lecturer.type != null).GroupBy(c => c.lecturer_id).Select(c => new
            {
                c.FirstOrDefault().lecturer.staff_id,
                full_name = c.FirstOrDefault().lecturer.full_name + " (" + c.FirstOrDefault().lecturer.type + ")",
                sum = c.Sum(item => item.total_lesson),
                theory_count = c.Count(item => item.type == "Lý thuyết"),
                practice_count = c.Count(item => item.type == "Thực hành"),
                curriculum_count = c.GroupBy(item => item.curriculum.id).Count(),
                curriculum_id = c.GroupBy(item => item.curriculum.id).Select(item => item.FirstOrDefault().curriculum_id),
                curriculum_name = c.GroupBy(item => item.curriculum.id).Select(item => item.FirstOrDefault().curriculum.name),
                curriculum_credits = c.GroupBy(item => item.curriculum.id).Select(item => item.FirstOrDefault().curriculum.credits),
                curriculum_major = c.GroupBy(item => item.curriculum.id).Select(item => item.FirstOrDefault().major.name),
                curriculum_hours = c.GroupBy(item => item.curriculum.id).Select(item => item.Sum(cu => cu.total_lesson))
            }).OrderByDescending(c => c.sum).ToList();
        }

        public IEnumerable GetYearStatistics(int startYear, int endYear, string lecturerType)
        {
            return context.curriculum_class.Where(c => c.term.start_year == startYear && c.term.end_year == endYear && c.lecturer.type == lecturerType).GroupBy(c => c.lecturer_id).Select(c => new
            {
                c.FirstOrDefault().lecturer.staff_id,
                c.FirstOrDefault().lecturer.full_name,
                sum = c.Sum(item => item.total_lesson),
                theory_count = c.Count(item => item.type == "Lý thuyết"),
                practice_count = c.Count(item => item.type == "Thực hành"),
                curriculum_count = c.GroupBy(item => item.curriculum.id).Count(),
                curriculum_id = c.GroupBy(item => item.curriculum.id).Select(item => item.FirstOrDefault().curriculum_id),
                curriculum_name = c.GroupBy(item => item.curriculum.id).Select(item => item.FirstOrDefault().curriculum.name),
                curriculum_credits = c.GroupBy(item => item.curriculum.id).Select(item => item.FirstOrDefault().curriculum.credits),
                curriculum_major = c.GroupBy(item => item.curriculum.id).Select(item => item.FirstOrDefault().major.name),
                curriculum_hours = c.GroupBy(item => item.curriculum.id).Select(item => item.Sum(cu => cu.total_lesson))
            }).OrderByDescending(c => c.sum).ToList();
        }

        public IEnumerable GetYearStatisticsAll(int startYear, int endYear)
        {
            return context.curriculum_class.Where(c => c.term.start_year == startYear && c.term.end_year == endYear && c.lecturer.type != null).GroupBy(c => c.lecturer_id).Select(c => new
            {
                c.FirstOrDefault().lecturer.staff_id,
                full_name = c.FirstOrDefault().lecturer.full_name + " (" + c.FirstOrDefault().lecturer.type + ")",
                sum = c.Sum(item => item.total_lesson),
                theory_count = c.Count(item => item.type == "Lý thuyết"),
                practice_count = c.Count(item => item.type == "Thực hành"),
                curriculum_count = c.GroupBy(item => item.curriculum.id).Count(),
                curriculum_id = c.GroupBy(item => item.curriculum.id).Select(item => item.FirstOrDefault().curriculum_id),
                curriculum_name = c.GroupBy(item => item.curriculum.id).Select(item => item.FirstOrDefault().curriculum.name),
                curriculum_credits = c.GroupBy(item => item.curriculum.id).Select(item => item.FirstOrDefault().curriculum.credits),
                curriculum_major = c.GroupBy(item => item.curriculum.id).Select(item => item.FirstOrDefault().major.name),
                curriculum_hours = c.GroupBy(item => item.curriculum.id).Select(item => item.Sum(cu => cu.total_lesson))
            }).OrderByDescending(c => c.sum).ToList();
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