using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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
                new major { id = "7480103", name = "Kỹ Thuật Phần Mềm", abbreviation = "KTPM" },
                new major { id = "7480201", name = "Công Nghệ Thông Tin", abbreviation = "CNTT" }
            }.AsQueryable();
            listLecturer = new List<lecturer> {
                new lecturer() { id = userId1, staff_id = "1001", full_name = "Nguyễn Văn A", type = "TG", status = true },
                new lecturer() { id = userId2, staff_id = "1002", full_name = "Nguyễn Văn B", type = "CH", status = true }
            }.AsQueryable();
            listSubject = new List<subject>
            {
                new subject() { id = "71ITBS10103", name = "Nhập môn Công nghệ thông tin", credits = 3 }
            }.AsQueryable();
            listClassSection = new List<class_section>
            {
                new class_section() { id = 1, class_section_id = "221_71ITBS10103_01", original_id = "221_71ITBS10103_01", type = "Lý thuyết", student_class_id = "71K28CNTT02 71K28CNTT03 71K28CNTT01", minimum_student = 60, total_lesson = 30, day = "Thứ Bảy", start_lesson = 4, lesson_number = 3, lesson_time = "4 - 6", student_number = 90, free_slot = 20, state = "Đang lập kế hoạch", learn_week = "07,08,09,10,11,12,13,14,15,16", day_2 = 7, start_lesson_2 = 4, student_registered_number = 0, start_week = 7, end_week = 16, note_1 = "Mi input 27/9", note_2 = null, lecturer1 = listLecturer.Last(), lecturer_id = listLecturer.First().id, lecturer = listLecturer.First(), term_id = termId, major_id = majorId, major = listMajor.First(), subject_id = listSubject.First().id, subject = listSubject.First(), room_id = "CS3.F.04.01" },
                new class_section() { id = 2, class_section_id = "221_71ITBS10103_02", original_id = "221_71ITBS10103_02", type = "Thực hành", student_class_id = "71K28CNTT02 71K28CNTT03 71K28CNTT01", minimum_student = 60, total_lesson = 30, day = "Thứ Bảy", start_lesson = 1, lesson_number = 3, lesson_time = "1 - 3", student_number = 90, free_slot = 20, state = "Đang lập kế hoạch", learn_week = "07,08,09,10,11,12,13,14,15,16", day_2 = 7, start_lesson_2 = 1, student_registered_number = 0, start_week = 7, end_week = 16, note_1 = "Mi input 27/9", note_2 = null, lecturer1 = listLecturer.Last(), lecturer_id = listLecturer.First().id, lecturer = listLecturer.First(), term_id = termId, major_id = majorId, major = listMajor.First(), subject_id = listSubject.First().id, subject = listSubject.First(), room_id = "CS3.F.04.01" }
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
    }
}