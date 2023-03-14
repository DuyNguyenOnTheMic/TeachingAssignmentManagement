using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TeachingAssignmentManagement.Helpers;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL.Tests
{
    [TestClass()]
    public class ClassSectionRepositoryTests
    {
        private IQueryable<term> listTerm;
        private IQueryable<major> listMajor;
        private IQueryable<lecturer> listLecturer;
        private IQueryable<subject> listSubject;
        private IQueryable<class_section> listClassSection;
        private Mock<DbSet<term>> mockSetTerm;
        private Mock<DbSet<major>> mockSetMajor;
        private Mock<DbSet<lecturer>> mockSetLecturer;
        private Mock<DbSet<class_section>> mockSetClassSection;
        private Mock<CP25Team03Entities> mockContext;
        private UnitOfWork unitOfWork;
        readonly int termId = 123;
        readonly string majorId = "7480103";
        readonly string userId1 = Guid.NewGuid().ToString();
        readonly string userId2 = Guid.NewGuid().ToString();

        [TestInitialize()]
        public void Initialize()
        {
            listTerm = new List<term> {
                new term() { id = 123, start_year = 2022, end_year = 2023, start_week = 1, start_date = DateTime.Now, max_lesson = 6, max_class = 6, status = true },
                new term() { id = 124, start_year = 2023, end_year = 2024, start_week = 1, start_date = DateTime.Now, max_lesson = 6, max_class = 6, status = true }
            }.AsQueryable();
            listMajor = new List<major> {
                new major() { id = "1", name = "Công Nghệ Thông Tin", abbreviation = "CNTT", program_type = MyConstants.StandardProgramType },
                new major() { id = "2", name = "Kỹ Thuật Phần Mềm", abbreviation = "KTPM", program_type = MyConstants.StandardProgramType }
            }.AsQueryable();
            listLecturer = new List<lecturer> {
                new lecturer() { id = userId1, staff_id = "1001", full_name = "Nguyễn Văn A", type = MyConstants.VisitingLecturerType, is_vietnamese = true, status = true },
                new lecturer() { id = userId2, staff_id = "1002", full_name = "Nguyễn Văn B", type = MyConstants.FacultyMemberType, is_vietnamese = true, status = true }
            }.AsQueryable();
            listSubject = new List<subject>
            {
                new subject() { id = "71ITBS10103", name = "Nhập môn Công nghệ thông tin", credits = 3, term_id = termId, major_id = majorId }
            }.AsQueryable();
            listClassSection = new List<class_section>
            {
                new class_section() { id = 1, class_section_id = "221_71ITBS10103_01", original_id = "221_71ITBS10103_01", type = MyConstants.TheoreticalClassType, student_class_id = "71K28CNTT02 71K28CNTT03 71K28CNTT01", minimum_student = 60, total_lesson = 30, day = "Thứ Bảy", start_lesson = 4, lesson_number = 3, lesson_time = "4 - 6", student_number = 90, free_slot = 20, state = "Đang lập kế hoạch", learn_week = "07,08,09,10,11,12,13,14,15,16", day_2 = 7, start_lesson_2 = 4, student_registered_number = 0, start_week = 7, end_week = 16, note_1 = "Mi input 27/9", note_2 = null, lecturer1 = listLecturer.Last(), lecturer_id = listLecturer.First().id, lecturer = listLecturer.First(), term_id = termId, major_id = majorId, major = listMajor.First(), subject_id = listSubject.First().id, subject = listSubject.First(), room_id = "CS3.F.04.01" },
                new class_section() { id = 2, class_section_id = "221_71ITBS10103_02", original_id = "221_71ITBS10103_02", type = MyConstants.PracticeClassType, student_class_id = "71K28CNTT02 71K28CNTT03 71K28CNTT01", minimum_student = 60, total_lesson = 30, day = "Thứ Bảy", start_lesson = 1, lesson_number = 3, lesson_time = "1 - 3", student_number = 90, free_slot = 20, state = "Đang lập kế hoạch", learn_week = "07,08,09,10,11,12,13,14,15,16", day_2 = 7, start_lesson_2 = 1, student_registered_number = 0, start_week = 7, end_week = 16, note_1 = "Mi input 27/9", note_2 = null, lecturer1 = listLecturer.Last(), lecturer_id = listLecturer.First().id, lecturer = listLecturer.First(), term_id = termId, major_id = majorId, major = listMajor.First(), subject_id = listSubject.First().id, subject = listSubject.First(), room_id = "CS3.F.04.01" }
            }.AsQueryable();
            mockSetTerm = new Mock<DbSet<term>>();
            mockSetMajor = new Mock<DbSet<major>>();
            mockSetLecturer = new Mock<DbSet<lecturer>>();
            mockSetClassSection = new Mock<DbSet<class_section>>();
            mockContext = new Mock<CP25Team03Entities>();
            unitOfWork = new UnitOfWork(mockContext.Object);
            mockSetTerm.As<IQueryable<term>>().Setup(m => m.Provider).Returns(listTerm.Provider);
            mockSetTerm.As<IQueryable<term>>().Setup(m => m.Expression).Returns(listTerm.Expression);
            mockSetTerm.As<IQueryable<term>>().Setup(m => m.ElementType).Returns(listTerm.ElementType);
            mockSetTerm.As<IQueryable<term>>().Setup(m => m.GetEnumerator()).Returns(listTerm.GetEnumerator());
            mockSetMajor.As<IQueryable<major>>().Setup(m => m.Provider).Returns(listMajor.Provider);
            mockSetMajor.As<IQueryable<major>>().Setup(m => m.Expression).Returns(listMajor.Expression);
            mockSetMajor.As<IQueryable<major>>().Setup(m => m.ElementType).Returns(listMajor.ElementType);
            mockSetMajor.As<IQueryable<major>>().Setup(m => m.GetEnumerator()).Returns(listMajor.GetEnumerator());
            mockSetLecturer.As<IQueryable<lecturer>>().Setup(m => m.Provider).Returns(listLecturer.Provider);
            mockSetLecturer.As<IQueryable<lecturer>>().Setup(m => m.Expression).Returns(listLecturer.Expression);
            mockSetLecturer.As<IQueryable<lecturer>>().Setup(m => m.ElementType).Returns(listLecturer.ElementType);
            mockSetLecturer.As<IQueryable<lecturer>>().Setup(m => m.GetEnumerator()).Returns(listLecturer.GetEnumerator());
            mockSetClassSection.As<IQueryable<class_section>>().Setup(m => m.Provider).Returns(listClassSection.Provider);
            mockSetClassSection.As<IQueryable<class_section>>().Setup(m => m.Expression).Returns(listClassSection.Expression);
            mockSetClassSection.As<IQueryable<class_section>>().Setup(m => m.ElementType).Returns(listClassSection.ElementType);
            mockSetClassSection.As<IQueryable<class_section>>().Setup(m => m.GetEnumerator()).Returns(listClassSection.GetEnumerator());
            mockContext.Setup(c => c.terms).Returns(() => mockSetTerm.Object);
            mockContext.Setup(c => c.majors).Returns(() => mockSetMajor.Object);
            mockContext.Setup(c => c.lecturers).Returns(() => mockSetLecturer.Object);
            mockContext.Setup(c => c.class_section).Returns(() => mockSetClassSection.Object);
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            unitOfWork.Dispose();
        }

        [TestMethod()]
        public void Get_Timetable_Data_Not_Null_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetTimetable(termId, userId1);

            // Assert
            Assert.IsNotNull(actionResult);
        }

        [TestMethod()]
        public void Get_Timetable_Data_Is_Correct_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetTimetable(termId, userId1).ToList();
            List<class_section> classSectionList = listClassSection.ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            for (int i = 0; i < classSectionList.Count; i++)
            {
                Assert.AreEqual(actionResult[i].ClassSectionId, classSectionList[i].class_section_id);
                Assert.AreEqual(actionResult[i].Type, classSectionList[i].type);
                Assert.AreEqual(actionResult[i].LessonTime, classSectionList[i].lesson_time);
                Assert.AreEqual(actionResult[i].Day2, classSectionList[i].day_2);
                Assert.AreEqual(actionResult[i].StartLesson2, classSectionList[i].start_lesson_2);
                Assert.AreEqual(actionResult[i].LearnWeek, classSectionList[i].learn_week);
                Assert.AreEqual(actionResult[i].StartWeek, classSectionList[i].start_week);
                Assert.AreEqual(actionResult[i].EndWeek, classSectionList[i].end_week);
                Assert.AreEqual(actionResult[i].SubjectId, classSectionList[i].subject_id);
                Assert.AreEqual(actionResult[i].RoomId, classSectionList[i].room_id);
                Assert.AreEqual(actionResult[i].Subject, classSectionList[i].subject);
            }
        }

        [TestMethod()]
        public void Timetable_Data_Should_Be_IEnumerable_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetTimetable(termId, userId1);
            int count = 0;
            foreach (dynamic value in actionResult)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Timetable_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetTimetable(termId, userId1).ToList();

            // Assert                
            Assert.IsNotNull(actionResult[0]);
        }

        [TestMethod()]
        public void Timetable_Data_Should_Be_Indexable_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetTimetable(termId, userId1).ToList();

            // Assert
            for (int i = 0; i < actionResult.Count; i++)
            {

                dynamic json = actionResult[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.ClassSectionId);
                Assert.IsNotNull(json.Type);
                Assert.IsNotNull(json.LessonTime);
                Assert.IsNotNull(json.Day2);
                Assert.IsNotNull(json.StartLesson2);
                Assert.IsNotNull(json.LearnWeek);
                Assert.IsNotNull(json.StartWeek);
                Assert.IsNotNull(json.EndWeek);
                Assert.IsNotNull(json.SubjectId);
                Assert.IsNotNull(json.RoomId);
                Assert.IsNotNull(json.Subject);
            }
        }

        [TestMethod()]
        public void Get_Timetable_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetTimetable(termId, userId1).ToList();
            List<class_section> query_classSection = listClassSection.Where(c => c.term_id == termId && c.lecturer_id == userId1).ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(query_classSection.Count(), actionResult.Count);
        }

        [TestMethod()]
        public void Get_Class_In_Week_Data_Not_Null_Test()
        {
            // Arrange
            int week = 7;

            // Act
            IEnumerable<ClassSectionDTO> query_classes = unitOfWork.ClassSectionRepository.GetTimetable(termId, userId1);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassInWeek(query_classes, week);

            // Assert
            Assert.IsNotNull(actionResult);
        }

        [TestMethod()]
        public void Get_Class_In_Week_Data_Is_Correct_Test()
        {
            // Arrange
            int week = 7;

            // Act
            IEnumerable<ClassSectionDTO> query_classes = unitOfWork.ClassSectionRepository.GetTimetable(termId, userId1);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassInWeek(query_classes, week).ToList();
            List<class_section> classSectionList = listClassSection.Where(c => c.term_id == termId && c.lecturer_id == userId1).ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            for (int i = 0; i < classSectionList.Count; i++)
            {
                Assert.AreEqual(actionResult[i].ClassSectionId, classSectionList[i].class_section_id);
                Assert.AreEqual(actionResult[i].Type, classSectionList[i].type);
                Assert.AreEqual(actionResult[i].LessonTime, classSectionList[i].lesson_time);
                Assert.AreEqual(actionResult[i].Day2, classSectionList[i].day_2);
                Assert.AreEqual(actionResult[i].StartLesson2, classSectionList[i].start_lesson_2);
                Assert.AreEqual(actionResult[i].LearnWeek, classSectionList[i].learn_week);
                Assert.AreEqual(actionResult[i].StartWeek, classSectionList[i].start_week);
                Assert.AreEqual(actionResult[i].EndWeek, classSectionList[i].end_week);
                Assert.AreEqual(actionResult[i].SubjectId, classSectionList[i].subject_id);
                Assert.AreEqual(actionResult[i].RoomId, classSectionList[i].room_id);
                Assert.AreEqual(actionResult[i].Subject, classSectionList[i].subject);
            }
        }

        [TestMethod()]
        public void Class_In_Week_Data_Should_Be_IEnumerable_Test()
        {
            // Arrange
            int week = 7;

            // Act
            IEnumerable<ClassSectionDTO> query_classes = unitOfWork.ClassSectionRepository.GetTimetable(termId, userId1);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassInWeek(query_classes, week);
            int count = 0;
            foreach (dynamic value in actionResult)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Class_In_Week_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Arrange
            int week = 7;

            // Act
            IEnumerable<ClassSectionDTO> query_classes = unitOfWork.ClassSectionRepository.GetTimetable(termId, userId1);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassInWeek(query_classes, week).ToList();

            // Assert                
            Assert.IsNotNull(actionResult[0]);
        }

        [TestMethod()]
        public void Class_In_Week_Data_Should_Be_Indexable_Test()
        {
            // Arrange
            int week = 7;

            // Act
            IEnumerable<ClassSectionDTO> query_classes = unitOfWork.ClassSectionRepository.GetTimetable(termId, userId1);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassInWeek(query_classes, week).ToList();

            // Assert
            for (int i = 0; i < actionResult.Count; i++)
            {

                dynamic json = actionResult[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.ClassSectionId);
                Assert.IsNotNull(json.Type);
                Assert.IsNotNull(json.LessonTime);
                Assert.IsNotNull(json.Day2);
                Assert.IsNotNull(json.StartLesson2);
                Assert.IsNotNull(json.LearnWeek);
                Assert.IsNotNull(json.StartWeek);
                Assert.IsNotNull(json.EndWeek);
                Assert.IsNotNull(json.SubjectId);
                Assert.IsNotNull(json.RoomId);
                Assert.IsNotNull(json.Subject);
            }
        }

        [TestMethod()]
        public void Get_Class_In_Week_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Arrange
            int week = 7;

            // Act
            IEnumerable<ClassSectionDTO> query_classes = unitOfWork.ClassSectionRepository.GetTimetable(termId, userId1);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassInWeek(query_classes, week).ToList();
            List<class_section> query_classSection = listClassSection.Where(c => c.term_id == termId && c.lecturer_id == userId1).ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(query_classSection.Count(), actionResult.Count);
        }

        [TestMethod()]
        public void Get_Class_In_Week_Data_Not_Null_When_Week_Have_Zero_Test()
        {
            // Arrange
            int week = 07;

            // Act
            IEnumerable<ClassSectionDTO> query_classes = unitOfWork.ClassSectionRepository.GetTimetable(termId, userId1);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassInWeek(query_classes, week);

            // Assert
            Assert.IsNotNull(actionResult);
        }

        [TestMethod()]
        public void Get_Class_In_Week_Data_Is_Correct_When_Week_Have_Zero_Test()
        {
            // Arrange
            int week = 07;

            // Act
            IEnumerable<ClassSectionDTO> query_classes = unitOfWork.ClassSectionRepository.GetTimetable(termId, userId1);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassInWeek(query_classes, week).ToList();
            List<class_section> classSectionList = listClassSection.Where(c => c.term_id == termId && c.lecturer_id == userId1).ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            for (int i = 0; i < classSectionList.Count; i++)
            {
                Assert.AreEqual(actionResult[i].ClassSectionId, classSectionList[i].class_section_id);
                Assert.AreEqual(actionResult[i].Type, classSectionList[i].type);
                Assert.AreEqual(actionResult[i].LessonTime, classSectionList[i].lesson_time);
                Assert.AreEqual(actionResult[i].Day2, classSectionList[i].day_2);
                Assert.AreEqual(actionResult[i].StartLesson2, classSectionList[i].start_lesson_2);
                Assert.AreEqual(actionResult[i].LearnWeek, classSectionList[i].learn_week);
                Assert.AreEqual(actionResult[i].StartWeek, classSectionList[i].start_week);
                Assert.AreEqual(actionResult[i].EndWeek, classSectionList[i].end_week);
                Assert.AreEqual(actionResult[i].SubjectId, classSectionList[i].subject_id);
                Assert.AreEqual(actionResult[i].RoomId, classSectionList[i].room_id);
                Assert.AreEqual(actionResult[i].Subject, classSectionList[i].subject);
            }
        }

        [TestMethod()]
        public void Class_In_Week_Data_Should_Be_IEnumerable_When_Week_Have_Zero_Test()
        {
            // Arrange
            int week = 07;

            // Act
            IEnumerable<ClassSectionDTO> query_classes = unitOfWork.ClassSectionRepository.GetTimetable(termId, userId1);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassInWeek(query_classes, week);
            int count = 0;
            foreach (dynamic value in actionResult)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Class_In_Week_Data_Index_at_0_Should_Not_Be_Null_When_Week_Have_Zero_Test()
        {
            // Arrange
            int week = 07;

            // Act
            IEnumerable<ClassSectionDTO> query_classes = unitOfWork.ClassSectionRepository.GetTimetable(termId, userId1);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassInWeek(query_classes, week).ToList();

            // Assert                
            Assert.IsNotNull(actionResult[0]);
        }

        [TestMethod()]
        public void Class_In_Week_Data_Should_Be_Indexable_When_Week_Have_Zero_Test()
        {
            // Arrange
            int week = 07;

            // Act
            IEnumerable<ClassSectionDTO> query_classes = unitOfWork.ClassSectionRepository.GetTimetable(termId, userId1);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassInWeek(query_classes, week).ToList();

            // Assert
            for (int i = 0; i < actionResult.Count; i++)
            {

                dynamic json = actionResult[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.ClassSectionId);
                Assert.IsNotNull(json.Type);
                Assert.IsNotNull(json.LessonTime);
                Assert.IsNotNull(json.Day2);
                Assert.IsNotNull(json.StartLesson2);
                Assert.IsNotNull(json.LearnWeek);
                Assert.IsNotNull(json.StartWeek);
                Assert.IsNotNull(json.EndWeek);
                Assert.IsNotNull(json.SubjectId);
                Assert.IsNotNull(json.RoomId);
                Assert.IsNotNull(json.Subject);
            }
        }

        [TestMethod()]
        public void Get_Class_In_Week_List_Should_Be_Not_Null_And_Equal_When_Week_Have_Zero_Test()
        {
            // Arrange
            int week = 07;

            // Act
            IEnumerable<ClassSectionDTO> query_classes = unitOfWork.ClassSectionRepository.GetTimetable(termId, userId1);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassInWeek(query_classes, week).ToList();
            List<class_section> query_classSection = listClassSection.Where(c => c.term_id == termId && c.lecturer_id == userId1).ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(query_classSection.Count(), actionResult.Count);
        }

        [TestMethod()]
        public void Get_Assign_Timetable_Data_Not_Null_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetAssignTimetable(termId, majorId);

            // Assert
            Assert.IsNotNull(actionResult);
        }

        [TestMethod()]
        public void Get_Assign_Timetable_Data_Is_Correct_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetAssignTimetable(termId, majorId).ToList();
            List<class_section> classSectionList = listClassSection.ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            for (int i = 0; i < classSectionList.Count; i++)
            {
                Assert.AreEqual(actionResult[i].Id, classSectionList[i].id);
                Assert.AreEqual(actionResult[i].ClassSectionId, classSectionList[i].class_section_id);
                Assert.AreEqual(actionResult[i].Type, classSectionList[i].type);
                Assert.AreEqual(actionResult[i].Day2, classSectionList[i].day_2);
                Assert.AreEqual(actionResult[i].StartLesson2, classSectionList[i].start_lesson_2);
                Assert.AreEqual(actionResult[i].StudentRegisteredNumber, classSectionList[i].student_registered_number);
                Assert.AreEqual(actionResult[i].LecturerId, classSectionList[i].lecturer_id);
                Assert.AreEqual(actionResult[i].LecturerName, classSectionList[i].lecturer.full_name);
                Assert.AreEqual(actionResult[i].SubjectId, classSectionList[i].subject_id);
                Assert.AreEqual(actionResult[i].RoomId, classSectionList[i].room_id);
                Assert.AreEqual(actionResult[i].Subject, classSectionList[i].subject);
            }
        }

        [TestMethod()]
        public void Assign_Timetable_Data_Should_Be_IEnumerable_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetAssignTimetable(termId, majorId).ToList();
            int count = 0;
            foreach (dynamic value in actionResult)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Assign_Timetable_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetAssignTimetable(termId, majorId).ToList();

            // Assert                
            Assert.IsNotNull(actionResult[0]);
        }

        [TestMethod()]
        public void Assign_Timetable_Data_Should_Be_Indexable_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetAssignTimetable(termId, majorId).ToList();

            // Assert
            for (int i = 0; i < actionResult.Count; i++)
            {

                dynamic json = actionResult[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.Id);
                Assert.IsNotNull(json.ClassSectionId);
                Assert.IsNotNull(json.Type);
                Assert.IsNotNull(json.Day2);
                Assert.IsNotNull(json.StartLesson2);
                Assert.IsNotNull(json.StudentRegisteredNumber);
                Assert.IsNotNull(json.LecturerId);
                Assert.IsNotNull(json.LecturerName);
                Assert.IsNotNull(json.SubjectId);
                Assert.IsNotNull(json.RoomId);
                Assert.IsNotNull(json.Subject);
            }
        }

        [TestMethod()]
        public void Get_Assign_Timetable_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetAssignTimetable(termId, majorId).ToList();
            List<class_section> query_classSection = listClassSection.Where(c => c.term_id == termId && c.major_id == majorId).ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(query_classSection.Count(), actionResult.Count);
        }

        [TestMethod()]
        public void Get_Term_Assign_Timetable_Data_Not_Null_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetTermAssignTimetable(termId);

            // Assert
            Assert.IsNotNull(actionResult);
        }

        [TestMethod()]
        public void Get_Term_Assign_Timetable_Data_Is_Correct_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetTermAssignTimetable(termId).ToList();
            List<class_section> classSectionList = listClassSection.ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            for (int i = 0; i < classSectionList.Count; i++)
            {
                Assert.AreEqual(actionResult[i].Id, classSectionList[i].id);
                Assert.AreEqual(actionResult[i].ClassSectionId, classSectionList[i].class_section_id);
                Assert.AreEqual(actionResult[i].Type, classSectionList[i].type);
                Assert.AreEqual(actionResult[i].Day2, classSectionList[i].day_2);
                Assert.AreEqual(actionResult[i].StartLesson2, classSectionList[i].start_lesson_2);
                Assert.AreEqual(actionResult[i].StudentRegisteredNumber, classSectionList[i].student_registered_number);
                Assert.AreEqual(actionResult[i].LecturerId, classSectionList[i].lecturer_id);
                Assert.AreEqual(actionResult[i].LecturerName, classSectionList[i].lecturer.full_name);
                Assert.AreEqual(actionResult[i].SubjectId, classSectionList[i].subject_id);
                Assert.AreEqual(actionResult[i].RoomId, classSectionList[i].room_id);
                Assert.AreEqual(actionResult[i].Subject, classSectionList[i].subject);
            }
        }

        [TestMethod()]
        public void Term_Assign_Timetable_Data_Should_Be_IEnumerable_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetTermAssignTimetable(termId).ToList();
            int count = 0;
            foreach (dynamic value in actionResult)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Term_Assign_Timetable_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetTermAssignTimetable(termId).ToList();

            // Assert                
            Assert.IsNotNull(actionResult[0]);
        }

        [TestMethod()]
        public void Term_Assign_Timetable_Data_Should_Be_Indexable_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetTermAssignTimetable(termId).ToList();

            // Assert
            for (int i = 0; i < actionResult.Count; i++)
            {

                dynamic json = actionResult[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.Id);
                Assert.IsNotNull(json.ClassSectionId);
                Assert.IsNotNull(json.Type);
                Assert.IsNotNull(json.Day2);
                Assert.IsNotNull(json.StartLesson2);
                Assert.IsNotNull(json.StudentRegisteredNumber);
                Assert.IsNotNull(json.LecturerId);
                Assert.IsNotNull(json.LecturerName);
                Assert.IsNotNull(json.SubjectId);
                Assert.IsNotNull(json.RoomId);
                Assert.IsNotNull(json.Subject);
            }
        }

        [TestMethod()]
        public void Get_Term_Assign_Timetable_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetTermAssignTimetable(termId).ToList();
            List<class_section> query_classSection = listClassSection.Where(c => c.term_id == termId).ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(query_classSection.Count(), actionResult.Count);
        }

        [TestMethod()]
        public void Get_Timetable_Statistics_Data_Not_Null_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetTimetableStatistics(termId);

            // Assert
            Assert.IsNotNull(actionResult);
        }

        [TestMethod()]
        public void Get_Timetable_Statistics_Data_Is_Correct_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetTimetableStatistics(termId).ToList();
            List<class_section> classSectionList = listClassSection.ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            for (int i = 0; i < classSectionList.Count; i++)
            {
                Assert.AreEqual(actionResult[i].ClassSectionId, classSectionList[i].class_section_id);
                Assert.AreEqual(actionResult[i].Type, classSectionList[i].type);
                Assert.AreEqual(actionResult[i].Day2, classSectionList[i].day_2);
                Assert.AreEqual(actionResult[i].StartLesson2, classSectionList[i].start_lesson_2);
                Assert.AreEqual(actionResult[i].LearnWeek, classSectionList[i].learn_week);
                Assert.AreEqual(actionResult[i].StartWeek, classSectionList[i].start_week);
                Assert.AreEqual(actionResult[i].EndWeek, classSectionList[i].end_week);
                Assert.AreEqual(actionResult[i].LecturerId, classSectionList[i].lecturer_id);
                Assert.AreEqual(actionResult[i].LecturerName, classSectionList[i].lecturer.full_name);
                Assert.AreEqual(actionResult[i].RoomId, classSectionList[i].room_id);
                Assert.AreEqual(actionResult[i].MajorName, classSectionList[i].major.name);
                Assert.AreEqual(actionResult[i].Subject, classSectionList[i].subject);
            }
        }

        [TestMethod()]
        public void Timetable_Statistics_Data_Should_Be_IEnumerable_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetTimetableStatistics(termId).ToList();
            int count = 0;
            foreach (dynamic value in actionResult)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Timetable_Statistics_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetTimetableStatistics(termId).ToList();

            // Assert                
            Assert.IsNotNull(actionResult[0]);
        }

        [TestMethod()]
        public void Timetable_Statistics_Data_Should_Be_Indexable_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetTimetableStatistics(termId).ToList();

            // Assert
            for (int i = 0; i < actionResult.Count; i++)
            {

                dynamic json = actionResult[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.ClassSectionId);
                Assert.IsNotNull(json.Type);
                Assert.IsNotNull(json.Day2);
                Assert.IsNotNull(json.StartLesson2);
                Assert.IsNotNull(json.LearnWeek);
                Assert.IsNotNull(json.StartWeek);
                Assert.IsNotNull(json.EndWeek);
                Assert.IsNotNull(json.LecturerId);
                Assert.IsNotNull(json.LecturerName);
                Assert.IsNotNull(json.RoomId);
                Assert.IsNotNull(json.MajorName);
                Assert.IsNotNull(json.Subject);
            }
        }

        [TestMethod()]
        public void Get_Timetable_Statistics_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetTimetableStatistics(termId).ToList();
            List<class_section> query_classSection = listClassSection.Where(c => c.term_id == termId).ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(query_classSection.Count(), actionResult.Count);
        }

        [TestMethod()]
        public void Get_Export_Data_Not_Null_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetExportData(termId, majorId);

            // Assert
            Assert.IsNotNull(actionResult);
        }

        [TestMethod()]
        public void Get_Export_Data_Is_Correct_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetExportData(termId, majorId).ToList();
            List<class_section> classSectionList = listClassSection.ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            for (int i = 0; i < classSectionList.Count; i++)
            {
                Assert.AreEqual(actionResult[i].class_section_id, classSectionList[i].class_section_id);
                Assert.AreEqual(actionResult[i].type, classSectionList[i].type);
                Assert.AreEqual(actionResult[i].day_2, classSectionList[i].day_2);
                Assert.AreEqual(actionResult[i].start_lesson_2, classSectionList[i].start_lesson_2);
                Assert.AreEqual(actionResult[i].learn_week, classSectionList[i].learn_week);
                Assert.AreEqual(actionResult[i].start_week, classSectionList[i].start_week);
                Assert.AreEqual(actionResult[i].end_week, classSectionList[i].end_week);
                Assert.AreEqual(actionResult[i].lecturer_id, classSectionList[i].lecturer_id);
                Assert.AreEqual(actionResult[i].lecturer.full_name, classSectionList[i].lecturer.full_name);
                Assert.AreEqual(actionResult[i].room_id, classSectionList[i].room_id);
                Assert.AreEqual(actionResult[i].major.name, classSectionList[i].major.name);
                Assert.AreEqual(actionResult[i].subject, classSectionList[i].subject);
            }
        }

        [TestMethod()]
        public void Export_Data_Should_Be_IEnumerable_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetExportData(termId, majorId).ToList();
            int count = 0;
            foreach (dynamic value in actionResult)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Export_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetExportData(termId, majorId).ToList();

            // Assert                
            Assert.IsNotNull(actionResult[0]);
        }

        [TestMethod()]
        public void Export_Data_Should_Be_Indexable_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetExportData(termId, majorId).ToList();

            // Assert
            for (int i = 0; i < actionResult.Count; i++)
            {

                dynamic json = actionResult[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.class_section_id);
                Assert.IsNotNull(json.type);
                Assert.IsNotNull(json.day_2);
                Assert.IsNotNull(json.start_lesson_2);
                Assert.IsNotNull(json.learn_week);
                Assert.IsNotNull(json.start_week);
                Assert.IsNotNull(json.end_week);
                Assert.IsNotNull(json.lecturer_id);
                Assert.IsNotNull(json.lecturer.full_name);
                Assert.IsNotNull(json.room_id);
                Assert.IsNotNull(json.major.name);
                Assert.IsNotNull(json.subject);
            }
        }

        [TestMethod()]
        public void Get_Export_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetExportData(termId, majorId).ToList();
            List<class_section> query_classSection = listClassSection.Where(c => c.term_id == termId && c.major_id == majorId).ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(query_classSection.Count(), actionResult.Count);
        }

        [TestMethod()]
        public void Get_Classes_In_Term_Not_Null_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInTerm(termId, userId1);

            // Assert
            Assert.IsNotNull(actionResult);
        }

        [TestMethod()]
        public void Get_Classes_In_Term_Data_Is_Correct_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInTerm(termId, userId1).ToList();
            List<class_section> classSectionList = listClassSection.ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            for (int i = 0; i < classSectionList.Count; i++)
            {
                Assert.AreEqual(actionResult[i].class_section_id, classSectionList[i].class_section_id);
                Assert.AreEqual(actionResult[i].type, classSectionList[i].type);
                Assert.AreEqual(actionResult[i].day_2, classSectionList[i].day_2);
                Assert.AreEqual(actionResult[i].start_lesson_2, classSectionList[i].start_lesson_2);
                Assert.AreEqual(actionResult[i].learn_week, classSectionList[i].learn_week);
                Assert.AreEqual(actionResult[i].start_week, classSectionList[i].start_week);
                Assert.AreEqual(actionResult[i].end_week, classSectionList[i].end_week);
                Assert.AreEqual(actionResult[i].lecturer_id, classSectionList[i].lecturer_id);
                Assert.AreEqual(actionResult[i].lecturer.full_name, classSectionList[i].lecturer.full_name);
                Assert.AreEqual(actionResult[i].room_id, classSectionList[i].room_id);
                Assert.AreEqual(actionResult[i].major.name, classSectionList[i].major.name);
                Assert.AreEqual(actionResult[i].subject, classSectionList[i].subject);
            }
        }

        [TestMethod()]
        public void Classes_In_Term_Data_Should_Be_IEnumerable_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInTerm(termId, userId1).ToList();
            int count = 0;
            foreach (dynamic value in actionResult)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Classes_In_Term_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInTerm(termId, userId1).ToList();

            // Assert                
            Assert.IsNotNull(actionResult[0]);
        }

        [TestMethod()]
        public void Classes_In_Term_Data_Should_Be_Indexable_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInTerm(termId, userId1).ToList();

            // Assert
            for (int i = 0; i < actionResult.Count; i++)
            {

                dynamic json = actionResult[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.class_section_id);
                Assert.IsNotNull(json.type);
                Assert.IsNotNull(json.day_2);
                Assert.IsNotNull(json.start_lesson_2);
                Assert.IsNotNull(json.learn_week);
                Assert.IsNotNull(json.start_week);
                Assert.IsNotNull(json.end_week);
                Assert.IsNotNull(json.lecturer_id);
                Assert.IsNotNull(json.lecturer.full_name);
                Assert.IsNotNull(json.room_id);
                Assert.IsNotNull(json.major.name);
                Assert.IsNotNull(json.subject);
            }
        }

        [TestMethod()]
        public void Get_Classes_In_Term_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInTerm(termId, userId1).ToList();
            List<class_section> query_classSection = listClassSection.Where(c => c.term_id == termId && c.major_id == majorId).ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(query_classSection.Count(), actionResult.Count);
        }

        [TestMethod()]
        public void Get_Classes_In_Lesson_Not_Null_Test()
        {
            // Act
            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInLesson(query_classes, 1);

            // Assert
            Assert.IsNotNull(actionResult);
        }

        [TestMethod()]
        public void Get_Classes_In_Lesson_Data_Is_Correct_Test()
        {
            // Act
            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInLesson(query_classes, 1).ToList();
            List<class_section> classSectionList = listClassSection.Where(c => c.start_lesson_2 == 1).ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            for (int i = 0; i < classSectionList.Count; i++)
            {
                Assert.AreEqual(actionResult[i].class_section_id, classSectionList[i].class_section_id);
                Assert.AreEqual(actionResult[i].type, classSectionList[i].type);
                Assert.AreEqual(actionResult[i].day_2, classSectionList[i].day_2);
                Assert.AreEqual(actionResult[i].start_lesson_2, classSectionList[i].start_lesson_2);
                Assert.AreEqual(actionResult[i].learn_week, classSectionList[i].learn_week);
                Assert.AreEqual(actionResult[i].start_week, classSectionList[i].start_week);
                Assert.AreEqual(actionResult[i].end_week, classSectionList[i].end_week);
                Assert.AreEqual(actionResult[i].lecturer_id, classSectionList[i].lecturer_id);
                Assert.AreEqual(actionResult[i].lecturer.full_name, classSectionList[i].lecturer.full_name);
                Assert.AreEqual(actionResult[i].room_id, classSectionList[i].room_id);
                Assert.AreEqual(actionResult[i].major.name, classSectionList[i].major.name);
                Assert.AreEqual(actionResult[i].subject, classSectionList[i].subject);
            }
        }

        [TestMethod()]
        public void Classes_In_Lesson_Data_Should_Be_IEnumerable_Test()
        {
            // Act
            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInLesson(query_classes, 1).ToList();
            int count = 0;
            foreach (dynamic value in actionResult)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Classes_In_Lesson_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Act
            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInLesson(query_classes, 1).ToList();

            // Assert                
            Assert.IsNotNull(actionResult[0]);
        }

        [TestMethod()]
        public void Classes_In_Lesson_Data_Should_Be_Indexable_Test()
        {
            // Act
            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInLesson(query_classes, 1).ToList();

            // Assert
            for (int i = 0; i < actionResult.Count; i++)
            {

                dynamic json = actionResult[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.class_section_id);
                Assert.IsNotNull(json.type);
                Assert.IsNotNull(json.day_2);
                Assert.IsNotNull(json.start_lesson_2);
                Assert.IsNotNull(json.learn_week);
                Assert.IsNotNull(json.start_week);
                Assert.IsNotNull(json.end_week);
                Assert.IsNotNull(json.lecturer_id);
                Assert.IsNotNull(json.lecturer.full_name);
                Assert.IsNotNull(json.room_id);
                Assert.IsNotNull(json.major.name);
                Assert.IsNotNull(json.subject);
            }
        }

        [TestMethod()]
        public void Get_Classes_In_Lesson_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Act
            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInLesson(query_classes, 1).ToList();
            List<class_section> query_classSection = listClassSection.Where(c => c.term_id == termId && c.start_lesson_2 == 1).ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(query_classSection.Count(), actionResult.Count);
        }

        [TestMethod()]
        public void Get_Classes_In_Campus_Should_Return_Empty_IEnumerable_Not_Null_Test()
        {
            // Arrange
            class_section classSection = listClassSection.First();

            // Act
            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInCampus(query_classes, 1, classSection.room_id);

            // Assert
            Assert.IsNotNull(actionResult);
        }


        [TestMethod()]
        public void Get_Classes_In_Campus_Data_Should_Return_Empty_IEnumerable_Is_Correct_Test()
        {
            // Arrange
            class_section classSection = listClassSection.First();

            // Act
            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInCampus(query_classes, 1, classSection.room_id).ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Count == 0);
        }

        [TestMethod()]
        public void Classes_In_Campus_Data_Should_Return_Empty_IEnumerable_Test()
        {
            // Arrange
            class_section classSection = listClassSection.First();

            // Act
            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInCampus(query_classes, 1, classSection.room_id).ToList();
            int count = 0;
            foreach (dynamic value in actionResult)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(actionResult.Count == 0);
        }

        [TestMethod()]
        public void Get_Classes_In_Campus_Should_Return_Empty_If_Not_In_The_Next_Lesson_Test()
        {
            // Arrange
            class_section classSection = listClassSection.First();

            // Act
            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInCampus(query_classes, 13, classSection.room_id);

            // Assert
            Assert.IsNotNull(actionResult);
        }

        [TestMethod()]
        public void Get_Classes_In_Campus_Data_Should_Return_Empty_IEnumerable_If_Not_In_The_Next_Lesson_Test()
        {
            // Arrange
            class_section classSection = listClassSection.First();

            // Act
            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInCampus(query_classes, 13, classSection.room_id).ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.IsTrue(actionResult.Count == 0);
        }

        [TestMethod()]
        public void Classes_In_Campus_Data_Should_Return_Empty_IEnumerable_If_Not_In_The_Next_Lesson_Test()
        {
            // Arrange
            class_section classSection = listClassSection.First();

            // Act
            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInCampus(query_classes, 13, classSection.room_id).ToList();
            int count = 0;
            foreach (dynamic value in actionResult)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(actionResult.Count == 0);
        }

        [TestMethod()]
        public void Get_Classes_In_Campus_Should_Return_Different_Campus_Classes_Not_Null_Test()
        {
            // Arrange
            string differentCampus = "CS4.F.04.01";

            // Act
            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInCampus(query_classes, 1, differentCampus).ToList();

            // Assert
            Assert.IsNotNull(actionResult);
        }

        [TestMethod()]
        public void Get_Classes_In_Campus_Should_Return_Different_Campus_Classes_Data_Is_Correct_Test()
        {
            // Arrange
            string differentCampus = "CS4.F.04.01";

            // Act
            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInCampus(query_classes, 1, differentCampus).ToList();
            List<class_section> classSectionList = listClassSection.Where(c => c.start_lesson_2 != 1).ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            for (int i = 0; i < classSectionList.Count; i++)
            {
                Assert.AreEqual(actionResult[i].class_section_id, classSectionList[i].class_section_id);
                Assert.AreEqual(actionResult[i].type, classSectionList[i].type);
                Assert.AreEqual(actionResult[i].day_2, classSectionList[i].day_2);
                Assert.AreEqual(actionResult[i].start_lesson_2, classSectionList[i].start_lesson_2);
                Assert.AreEqual(actionResult[i].learn_week, classSectionList[i].learn_week);
                Assert.AreEqual(actionResult[i].start_week, classSectionList[i].start_week);
                Assert.AreEqual(actionResult[i].end_week, classSectionList[i].end_week);
                Assert.AreEqual(actionResult[i].lecturer_id, classSectionList[i].lecturer_id);
                Assert.AreEqual(actionResult[i].lecturer.full_name, classSectionList[i].lecturer.full_name);
                Assert.AreEqual(actionResult[i].room_id, classSectionList[i].room_id);
                Assert.AreEqual(actionResult[i].major.name, classSectionList[i].major.name);
                Assert.AreEqual(actionResult[i].subject, classSectionList[i].subject);
            }
        }

        [TestMethod()]
        public void Classes_In_Campus_Should_Return_Different_Campus_Classes_Data_Should_Be_IEnumerable_Test()
        {
            // Arrange
            string differentCampus = "CS4.F.04.01";

            // Act
            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInCampus(query_classes, 1, differentCampus).ToList();
            int count = 0;
            foreach (dynamic value in actionResult)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Classes_In_Campus_Should_Return_Different_Campus_Classes_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Arrange
            string differentCampus = "CS4.F.04.01";

            // Act
            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInCampus(query_classes, 1, differentCampus).ToList();

            // Assert                
            Assert.IsNotNull(actionResult[0]);
        }

        [TestMethod()]
        public void Classes_In_Campus_Should_Return_Different_Campus_Classes_Data_Should_Be_Indexable_Test()
        {
            // Arrange
            string differentCampus = "CS4.F.04.01";

            // Act
            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInCampus(query_classes, 1, differentCampus).ToList();

            // Assert
            for (int i = 0; i < actionResult.Count; i++)
            {

                dynamic json = actionResult[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.class_section_id);
                Assert.IsNotNull(json.type);
                Assert.IsNotNull(json.day_2);
                Assert.IsNotNull(json.start_lesson_2);
                Assert.IsNotNull(json.learn_week);
                Assert.IsNotNull(json.start_week);
                Assert.IsNotNull(json.end_week);
                Assert.IsNotNull(json.lecturer_id);
                Assert.IsNotNull(json.lecturer.full_name);
                Assert.IsNotNull(json.room_id);
                Assert.IsNotNull(json.major.name);
                Assert.IsNotNull(json.subject);
            }
        }

        [TestMethod()]
        public void Get_Classes_In_Campus_Should_Return_Different_Campus_Classes_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Arrange
            string differentCampus = "CS4.F.04.01";

            // Act
            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInCampus(query_classes, 1, differentCampus).ToList();
            List<class_section> query_classSection = listClassSection.Where(c => c.term_id == termId && c.start_lesson_2 != 1).ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(query_classSection.Count(), actionResult.Count);
        }

        [TestMethod()]
        public void Get_Classes_In_Day_Not_Null_Test()
        {
            // Act
            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);
            int day2 = listClassSection.FirstOrDefault().day_2;
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInDay(query_classes, day2);

            // Assert
            Assert.IsNotNull(actionResult);
        }

        [TestMethod()]
        public void Get_Classes_In_Day_Data_Is_Correct_Test()
        {
            // Act
            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);
            int day2 = listClassSection.FirstOrDefault().day_2;
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInDay(query_classes, day2).ToList();
            List<class_section> classSectionList = listClassSection.Where(c => c.start_lesson_2 == day2).ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            for (int i = 0; i < classSectionList.Count; i++)
            {
                Assert.AreEqual(actionResult[i].class_section_id, classSectionList[i].class_section_id);
                Assert.AreEqual(actionResult[i].type, classSectionList[i].type);
                Assert.AreEqual(actionResult[i].day_2, classSectionList[i].day_2);
                Assert.AreEqual(actionResult[i].start_lesson_2, classSectionList[i].start_lesson_2);
                Assert.AreEqual(actionResult[i].learn_week, classSectionList[i].learn_week);
                Assert.AreEqual(actionResult[i].start_week, classSectionList[i].start_week);
                Assert.AreEqual(actionResult[i].end_week, classSectionList[i].end_week);
                Assert.AreEqual(actionResult[i].lecturer_id, classSectionList[i].lecturer_id);
                Assert.AreEqual(actionResult[i].lecturer.full_name, classSectionList[i].lecturer.full_name);
                Assert.AreEqual(actionResult[i].room_id, classSectionList[i].room_id);
                Assert.AreEqual(actionResult[i].major.name, classSectionList[i].major.name);
                Assert.AreEqual(actionResult[i].subject, classSectionList[i].subject);
            }
        }

        [TestMethod()]
        public void Classes_In_Day_Data_Should_Be_IEnumerable_Test()
        {
            // Act
            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);
            int day2 = listClassSection.FirstOrDefault().day_2;
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInDay(query_classes, day2).ToList();
            int count = 0;
            foreach (dynamic value in actionResult)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Classes_In_Day_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Act
            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);
            int day2 = listClassSection.FirstOrDefault().day_2;
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInDay(query_classes, day2).ToList();

            // Assert                
            Assert.IsNotNull(actionResult[0]);
        }

        [TestMethod()]
        public void Classes_In_Day_Data_Should_Be_Indexable_Test()
        {
            // Act
            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);
            int day2 = listClassSection.FirstOrDefault().day_2;
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInDay(query_classes, day2).ToList();

            // Assert
            for (int i = 0; i < actionResult.Count; i++)
            {

                dynamic json = actionResult[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.class_section_id);
                Assert.IsNotNull(json.type);
                Assert.IsNotNull(json.day_2);
                Assert.IsNotNull(json.start_lesson_2);
                Assert.IsNotNull(json.learn_week);
                Assert.IsNotNull(json.start_week);
                Assert.IsNotNull(json.end_week);
                Assert.IsNotNull(json.lecturer_id);
                Assert.IsNotNull(json.lecturer.full_name);
                Assert.IsNotNull(json.room_id);
                Assert.IsNotNull(json.major.name);
                Assert.IsNotNull(json.subject);
            }
        }

        [TestMethod()]
        public void Get_Classes_In_Day_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Act
            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);
            int day2 = listClassSection.FirstOrDefault().day_2;
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesInDay(query_classes, day2).ToList();
            List<class_section> query_classSection = listClassSection.Where(c => c.term_id == termId && c.day_2 == day2).ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(query_classSection.Count(), actionResult.Count);
        }

        [TestMethod()]
        public void Get_Classes_By_Term_Not_Null_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);

            // Assert
            Assert.IsNotNull(actionResult);
        }

        [TestMethod()]
        public void Get_Classes_By_Term_Data_Is_Correct_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId).ToList();
            List<class_section> classSectionList = listClassSection.ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            for (int i = 0; i < classSectionList.Count; i++)
            {
                Assert.AreEqual(actionResult[i].class_section_id, classSectionList[i].class_section_id);
                Assert.AreEqual(actionResult[i].type, classSectionList[i].type);
                Assert.AreEqual(actionResult[i].day_2, classSectionList[i].day_2);
                Assert.AreEqual(actionResult[i].start_lesson_2, classSectionList[i].start_lesson_2);
                Assert.AreEqual(actionResult[i].learn_week, classSectionList[i].learn_week);
                Assert.AreEqual(actionResult[i].start_week, classSectionList[i].start_week);
                Assert.AreEqual(actionResult[i].end_week, classSectionList[i].end_week);
                Assert.AreEqual(actionResult[i].lecturer_id, classSectionList[i].lecturer_id);
                Assert.AreEqual(actionResult[i].lecturer.full_name, classSectionList[i].lecturer.full_name);
                Assert.AreEqual(actionResult[i].room_id, classSectionList[i].room_id);
                Assert.AreEqual(actionResult[i].major.name, classSectionList[i].major.name);
                Assert.AreEqual(actionResult[i].subject, classSectionList[i].subject);
            }
        }

        [TestMethod()]
        public void Classes_By_Term_Data_Should_Be_IEnumerable_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId).ToList();
            int count = 0;
            foreach (dynamic value in actionResult)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Classes_By_Term_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId).ToList();

            // Assert                
            Assert.IsNotNull(actionResult[0]);
        }

        [TestMethod()]
        public void Classes_By_Term_Data_Should_Be_Indexable_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId).ToList();

            // Assert
            for (int i = 0; i < actionResult.Count; i++)
            {

                dynamic json = actionResult[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.class_section_id);
                Assert.IsNotNull(json.type);
                Assert.IsNotNull(json.day_2);
                Assert.IsNotNull(json.start_lesson_2);
                Assert.IsNotNull(json.learn_week);
                Assert.IsNotNull(json.start_week);
                Assert.IsNotNull(json.end_week);
                Assert.IsNotNull(json.lecturer_id);
                Assert.IsNotNull(json.lecturer.full_name);
                Assert.IsNotNull(json.room_id);
                Assert.IsNotNull(json.major.name);
                Assert.IsNotNull(json.subject);
            }
        }

        [TestMethod()]
        public void Get_Classes_By_Term_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId).ToList();
            List<class_section> query_classSection = listClassSection.Where(c => c.term_id == termId).ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(query_classSection.Count(), actionResult.Count);
        }

        [TestMethod()]
        public void Find_Class_Section_Correctly_Test()
        {
            // Arrange
            class_section classSection = listClassSection.First();
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            class_section actionResult = unitOfWork.ClassSectionRepository.GetClassByID(classSection.id);

            // Assert
            mockSetClassSection.Verify(x => x.Find(It.IsAny<int>()), Times.Once());
            Assert.AreEqual(classSection.class_section_id, actionResult.class_section_id);
            Assert.AreEqual(classSection.type, actionResult.type);
            Assert.AreEqual(classSection.day_2, actionResult.day_2);
            Assert.AreEqual(classSection.start_lesson_2, actionResult.start_lesson_2);
            Assert.AreEqual(classSection.learn_week, actionResult.learn_week);
            Assert.AreEqual(classSection.start_week, actionResult.start_week);
            Assert.AreEqual(classSection.end_week, actionResult.end_week);
            Assert.AreEqual(classSection.lecturer_id, actionResult.lecturer_id);
            Assert.AreEqual(classSection.lecturer.full_name, actionResult.lecturer.full_name);
            Assert.AreEqual(classSection.room_id, actionResult.room_id);
            Assert.AreEqual(classSection.major.name, actionResult.major.name);
            Assert.AreEqual(classSection.subject, actionResult.subject);
        }

        [TestMethod()]
        public void Get_Term_And_Major_Statistics_Not_Null_Test()
        {
            // Arrange
            bool isLesson = false;

            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetTermStatistics(isLesson, termId, majorId, MyConstants.VisitingLecturerType);

            // Assert
            Assert.IsNotNull(actionResult);
        }

        [TestMethod()]
        public void Term_And_Major_Statistics_Data_Should_Be_IEnumerable_Test()
        {
            // Arrange
            bool isLesson = false;

            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetTermStatistics(isLesson, termId, majorId, MyConstants.VisitingLecturerType);
            int count = 0;
            foreach (dynamic value in actionResult)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Term_And_Major_Statistics_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Arrange
            bool isLesson = false;

            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetTermStatistics(isLesson, termId, majorId, MyConstants.VisitingLecturerType);

            // Assert                
            Assert.IsNotNull(actionResult[0]);
        }

        [TestMethod()]
        public void Term_And_Major_Statistics_Data_Should_Be_Indexable_Test()
        {
            // Arrange
            bool isLesson = false;

            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetTermStatistics(isLesson, termId, majorId, MyConstants.VisitingLecturerType);

            // Assert
            for (int i = 0; i < actionResult.Count; i++)
            {

                dynamic json = actionResult[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.Key);
                Assert.IsNotNull(json.staff_id);
                Assert.IsNotNull(json.full_name);
                Assert.IsNotNull(json.subject_count);
                Assert.IsNotNull(json.class_count);
                Assert.IsNotNull(json.sum);
                Assert.IsNotNull(json.lecturer_type);
            }
        }

        [TestMethod()]
        public void Get_Term_And_Major_Statistics_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Arrange
            bool isLesson = false;

            // Act
            dynamic actionResult = unitOfWork.ClassSectionRepository.GetTermStatistics(isLesson, termId, majorId, MyConstants.VisitingLecturerType);
            IQueryable<IGrouping<string, class_section>> query_classSection = listClassSection.Where(c => c.term_id == termId && c.major_id == majorId && c.lecturer.type == MyConstants.VisitingLecturerType).GroupBy(c => c.lecturer_id);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(query_classSection.Count(), actionResult.Count);
        }
    }
}