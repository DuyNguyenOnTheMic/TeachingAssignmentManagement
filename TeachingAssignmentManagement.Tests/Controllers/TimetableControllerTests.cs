﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Controllers.Tests
{
    [TestClass()]
    public class TimetableControllerTests
    {
        private IQueryable<lecturer> listLecturer;
        private IQueryable<subject> listSubject;
        private IQueryable<class_section> listClassSection;
        private IQueryable<term> listTerm;
        private Mock<DbSet<lecturer>> mockSetLecturer;
        private Mock<DbSet<class_section>> mockSetClassSection;
        private Mock<DbSet<term>> mockSetTerm;
        private Mock<CP25Team03Entities> mockContext;
        private UnitOfWork unitOfWork;
        readonly int termId = 123;
        readonly string majorId = "7480103";
        readonly string userId1 = Guid.NewGuid().ToString();
        readonly string userId2 = Guid.NewGuid().ToString();

        [TestInitialize()]
        public void Initialize()
        {
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
                new class_section() { id = 1, class_section_id = "221_71ITBS10103_01", original_id = "221_71ITBS10103_01", type = "Lý thuyết", student_class_id = "71K28CNTT02 71K28CNTT03 71K28CNTT01", minimum_student = 60, total_lesson = 30, day = "Thứ Bảy", start_lesson = 1, lesson_number = 3, lesson_time = "1 - 3", student_number = 90, free_slot = 20, state = "Đang lập kế hoạch", learn_week = "07,08,09,10,11,12,13,14,15,16", day_2 = 7, start_lesson_2 = 1, student_registered_number = 0, start_week = 7, end_week = 16, note_1 = "Mi input 27/9", note_2 = null, lecturer1 = listLecturer.First(), lecturer = listLecturer.Last(), term_id = termId, major_id = majorId, subject = listSubject.First(), room_id = "CS3.F.04.01" }
            }.AsQueryable();
            listTerm = new List<term> {
                new term() { id = 123, start_year = 2022, end_year = 2023, start_week = 1, start_date = DateTime.Now, max_lesson = 6, max_class = 6, status = true },
                new term() { id = 124, start_year = 2023, end_year = 2024, start_week = 1, start_date = DateTime.Now, max_lesson = 6, max_class = 6, status = true }
            }.AsQueryable();
            mockSetLecturer = new Mock<DbSet<lecturer>>();
            mockSetClassSection = new Mock<DbSet<class_section>>();
            mockSetTerm = new Mock<DbSet<term>>();
            mockContext = new Mock<CP25Team03Entities>();
            unitOfWork = new UnitOfWork(mockContext.Object);
            mockSetLecturer.As<IQueryable<lecturer>>().Setup(m => m.Provider).Returns(listLecturer.Provider);
            mockSetLecturer.As<IQueryable<lecturer>>().Setup(m => m.Expression).Returns(listLecturer.Expression);
            mockSetLecturer.As<IQueryable<lecturer>>().Setup(m => m.ElementType).Returns(listLecturer.ElementType);
            mockSetLecturer.As<IQueryable<lecturer>>().Setup(m => m.GetEnumerator()).Returns(listLecturer.GetEnumerator());
            mockSetClassSection.As<IQueryable<class_section>>().Setup(m => m.Provider).Returns(listClassSection.Provider);
            mockSetClassSection.As<IQueryable<class_section>>().Setup(m => m.Expression).Returns(listClassSection.Expression);
            mockSetClassSection.As<IQueryable<class_section>>().Setup(m => m.ElementType).Returns(listClassSection.ElementType);
            mockSetClassSection.As<IQueryable<class_section>>().Setup(m => m.GetEnumerator()).Returns(listClassSection.GetEnumerator());
            mockSetTerm.As<IQueryable<term>>().Setup(m => m.Provider).Returns(listTerm.Provider);
            mockSetTerm.As<IQueryable<term>>().Setup(m => m.Expression).Returns(listTerm.Expression);
            mockSetTerm.As<IQueryable<term>>().Setup(m => m.ElementType).Returns(listTerm.ElementType);
            mockSetTerm.As<IQueryable<term>>().Setup(m => m.GetEnumerator()).Returns(listTerm.GetEnumerator());
            mockContext.Setup(c => c.lecturers).Returns(() => mockSetLecturer.Object);
            mockContext.Setup(c => c.class_section).Returns(() => mockSetClassSection.Object);
            mockContext.Setup(c => c.terms).Returns(() => mockSetTerm.Object);
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            unitOfWork.Dispose();
        }

        [TestMethod()]
        public void Index_View_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void Index_View_Should_Load_Term_SelectList_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);

            // Act
            ViewResult result = controller.Index() as ViewResult;
            SelectList termList = new SelectList(listTerm);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(termList.Count(), ((IEnumerable<dynamic>)result.ViewBag.term).Count());
        }

        [TestMethod()]
        public void Index_View_Should_Load_Term_SelectList_Data_Correctly_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);

            // Act
            ViewResult result = controller.Index() as ViewResult;
            dynamic viewBagResult = result.ViewBag.term.Items;
            List<term> termList = listTerm.OrderByDescending(t => t.id).ToList();

            // Assert
            Assert.IsNotNull(result);
            for (int i = 0; i < termList.Count(); i++)
            {
                Assert.AreEqual(viewBagResult[i].id, termList[i].id);
                Assert.AreEqual(viewBagResult[i].start_year, termList[i].start_year);
            }
        }

        [TestMethod()]
        public void Get_Data_Partial_View_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);

            // Act
            PartialViewResult result = controller.GetData(termId, majorId) as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void Get_Data_Partial_View_Should_Load_Term_Status_Correctly_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);

            // Act
            PartialViewResult result = controller.GetData(termId, majorId) as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(term.status, result.ViewBag.termStatus);
        }

        [TestMethod()]
        public void Get_Data_Partial_View_Should_Load_Subject_Data_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);

            // Act
            PartialViewResult result = controller.GetData(termId, majorId) as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(listSubject.Count(), ((IEnumerable<dynamic>)result.ViewBag.subjects).Count());
        }

        [TestMethod()]
        public void Get_Data_Partial_View_Should_Load_Subject_Data_Correctly_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);

            // Act
            PartialViewResult result = controller.GetData(termId, majorId) as PartialViewResult;
            dynamic viewBagResult = result.ViewBag.subjects;
            List<subject> subjectList = listSubject.ToList();

            // Assert
            Assert.IsNotNull(result);
            for (int i = 0; i < subjectList.Count(); i++)
            {
                Assert.AreEqual(viewBagResult[i].id, subjectList[i].id);
                Assert.AreEqual(viewBagResult[i].name, subjectList[i].name);
                Assert.AreEqual(viewBagResult[i].credits, subjectList[i].credits);
            }
        }

        [TestMethod()]
        public void Get_Data_Partial_View_Should_Load_Lecturer_SelectList_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);

            // Act
            PartialViewResult result = controller.GetData(termId, majorId) as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(listLecturer.Count(), ((IEnumerable<dynamic>)result.ViewBag.lecturers).Count());
        }

        [TestMethod()]
        public void Get_Data_Partial_View_Should_Load_Lecturer_Data_Correctly_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);

            // Act
            PartialViewResult result = controller.GetData(termId, majorId) as PartialViewResult;
            dynamic viewBagResult = result.ViewBag.lecturers.Items;
            List<lecturer> lecturerList = listLecturer.ToList();

            // Assert
            Assert.IsNotNull(result);
            for (int i = 0; i < lecturerList.Count(); i++)
            {
                Assert.AreEqual(viewBagResult[i].id, lecturerList[i].id);
                Assert.AreEqual(viewBagResult[i].staff_id, lecturerList[i].staff_id);
                Assert.AreEqual(viewBagResult[i].full_name, lecturerList[i].full_name);
                Assert.AreEqual(viewBagResult[i].type, lecturerList[i].type);
                Assert.AreEqual(viewBagResult[i].status, lecturerList[i].status);
            }
        }
    }
}