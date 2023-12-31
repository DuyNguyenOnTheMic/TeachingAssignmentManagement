﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Helpers;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Controllers.Tests
{
    [TestClass()]
    public class StatisticsControllerTests
    {
        private IQueryable<term> listTerm;
        private IQueryable<major> listMajor;
        private ICollection<AspNetRole> listRole;
        private IQueryable<AspNetUser> listUser;
        private IQueryable<lecturer> listLecturer;
        private IQueryable<subject> listSubject;
        private IQueryable<class_section> listClassSection;
        private Mock<DbSet<term>> mockSetTerm;
        private Mock<DbSet<major>> mockSetMajor;
        private Mock<DbSet<AspNetUser>> mockSetUser;
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
                new major { id = "7480103", name = "Kỹ Thuật Phần Mềm", abbreviation = "KTPM", program_type = MyConstants.StandardProgramType },
                new major { id = "7480201", name = "Công Nghệ Thông Tin", abbreviation = "CNTT", program_type = MyConstants.StandardProgramType }
            }.AsQueryable();
            listRole = new List<AspNetRole> {
                new AspNetRole() { Id = "1", Name = "BCN khoa" }
            };
            listUser = new List<AspNetUser> {
                new AspNetUser() { Id = userId1, Email = "a.nv@vanlanguni.vn", UserName = "a.nv@vanlanguni.vn", AspNetRoles = listRole },
                new AspNetUser() { Id = userId2, Email = "b.nv@vlu.edu.vn", UserName = "b.nv@vlu.edu.vn", AspNetRoles = listRole }
            }.AsQueryable();
            listLecturer = new List<lecturer> {
                new lecturer() { id = userId1, staff_id = "1001", full_name = "Nguyễn Văn A", type = MyConstants.VisitingLecturerType, is_vietnamese = true, status = true },
                new lecturer() { id = userId2, staff_id = "1002", full_name = "Nguyễn Văn B", type = MyConstants.FacultyMemberType, is_vietnamese = true, status = true }
            }.AsQueryable();
            listSubject = new List<subject>
            {
                new subject() { id = "1", subject_id = "71ITBS10103", name = "Nhập môn Công nghệ thông tin", credits = 3, term_id = termId, major_id = majorId }
            }.AsQueryable();
            listClassSection = new List<class_section>
            {
                new class_section() { id = 1, class_section_id = "221_71ITBS10103_01", original_id = "221_71ITBS10103_01", type = MyConstants.TheoreticalClassType, student_class_id = "71K28CNTT02 71K28CNTT03 71K28CNTT01", minimum_student = 60, total_lesson = 30, day = "Thứ Bảy", start_lesson = 4, lesson_number = 3, lesson_time = "4 - 6", student_number = 90, free_slot = 20, state = "Đang lập kế hoạch", learn_week = "07,08,09,10,11,12,13,14,15,16", day_2 = 7, start_lesson_2 = 4, student_registered_number = 0, start_week = 7, end_week = 16, note_1 = "Mi input 27/9", note_2 = null, lecturer1 = listLecturer.Last(), lecturer_id = listLecturer.First().id, lecturer = listLecturer.First(), term = listTerm.First(), term_id = termId, major_id = majorId, major = listMajor.First(), subject_id = listSubject.First().id, subject = listSubject.First(), room_id = "CS3.F.04.01" },
                new class_section() { id = 2, class_section_id = "221_71ITBS10103_02", original_id = "221_71ITBS10103_02", type = MyConstants.PracticeClassType, student_class_id = "71K28CNTT02 71K28CNTT03 71K28CNTT01", minimum_student = 60, total_lesson = 30, day = "Thứ Bảy", start_lesson = 1, lesson_number = 3, lesson_time = "1 - 3", student_number = 90, free_slot = 20, state = "Đang lập kế hoạch", learn_week = "07,08,09,10,11,12,13,14,15,16", day_2 = 7, start_lesson_2 = 1, student_registered_number = 0, start_week = 7, end_week = 16, note_1 = "Mi input 27/9", note_2 = null, lecturer1 = listLecturer.Last(), lecturer_id = listLecturer.First().id, lecturer = listLecturer.First(), term = listTerm.First(), term_id = termId, major_id = majorId, major = listMajor.First(), subject_id = listSubject.First().id, subject = listSubject.First(), room_id = "CS3.F.04.01" }
            }.AsQueryable();
            mockSetTerm = new Mock<DbSet<term>>();
            mockSetMajor = new Mock<DbSet<major>>();
            mockSetUser = new Mock<DbSet<AspNetUser>>();
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
            mockSetUser.As<IQueryable<AspNetUser>>().Setup(m => m.Provider).Returns(listUser.Provider);
            mockSetUser.As<IQueryable<AspNetUser>>().Setup(m => m.Expression).Returns(listUser.Expression);
            mockSetUser.As<IQueryable<AspNetUser>>().Setup(m => m.ElementType).Returns(listUser.ElementType);
            mockSetUser.As<IQueryable<AspNetUser>>().Setup(m => m.GetEnumerator()).Returns(listUser.GetEnumerator());
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
            mockContext.Setup(c => c.AspNetUsers).Returns(() => mockSetUser.Object);
            mockContext.Setup(c => c.lecturers).Returns(() => mockSetLecturer.Object);
            mockContext.Setup(c => c.class_section).Returns(() => mockSetClassSection.Object);
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
            StatisticsController controller = new StatisticsController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void Index_View_Should_Load_Term_SelectList_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);

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
            StatisticsController controller = new StatisticsController(unitOfWork);

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
        public void Index_View_Should_Load_Year_SelectList_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);

            // Act
            ViewResult result = controller.Index() as ViewResult;
            SelectList termList = new SelectList(listTerm);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(termList.Count(), ((IEnumerable<dynamic>)result.ViewBag.year).Count());
        }

        [TestMethod()]
        public void Index_View_Should_Load_year_SelectList_Data_Correctly_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);

            // Act
            ViewResult result = controller.Index() as ViewResult;
            dynamic viewBagResult = result.ViewBag.year.Items;
            List<term> termList = listTerm.OrderByDescending(t => t.id).ToList();

            // Assert
            Assert.IsNotNull(result);
            for (int i = 0; i < termList.Count(); i++)
            {
                Assert.AreEqual(viewBagResult[i].schoolyear, termList[i].start_year + " - " + termList[i].end_year);
            }
        }

        [TestMethod()]
        public void Index_View_Should_Load_Major_SelectList_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);

            // Act
            ViewResult result = controller.Index() as ViewResult;
            SelectList majorList = new SelectList(listMajor);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(majorList.Count(), ((IEnumerable<dynamic>)result.ViewBag.major).Count());
        }

        [TestMethod()]
        public void Index_View_Should_Load_Major_SelectList_Data_Correctly_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);

            // Act
            ViewResult result = controller.Index() as ViewResult;
            dynamic viewBagResult = result.ViewBag.major.Items;
            List<major> majorList = listMajor.OrderByDescending(m => m.name.Contains("công nghệ thông tin")).ToList();

            // Assert
            Assert.IsNotNull(result);
            for (int i = 0; i < majorList.Count(); i++)
            {
                Assert.AreEqual(viewBagResult[i].id, majorList[i].id);
                Assert.AreEqual(viewBagResult[i].name, majorList[i].name);
                Assert.AreEqual(viewBagResult[i].abbreviation, majorList[i].abbreviation);
                Assert.AreEqual(viewBagResult[i].program_type, majorList[i].program_type);
            }
        }

        [TestMethod()]
        public void Get_Chart_View_Not_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController();
            bool isLesson = false;
            string type = "term";
            string value = termId.ToString();
            string major = majorId;
            string lecturerType = MyConstants.VisitingLecturerType;

            // Act
            PartialViewResult result = controller.GetChart(isLesson, type, value, major, lecturerType) as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void Get_Chart_View_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController();
            bool isLesson = false;
            string type = "term";
            string value = termId.ToString();
            string major = majorId;
            string lecturerType = MyConstants.VisitingLecturerType;

            // Act
            PartialViewResult result = controller.GetChart(isLesson, type, value, major, lecturerType) as PartialViewResult;

            // Assert
            Assert.AreEqual("_Chart", result.ViewName);
        }

        [TestMethod()]
        public void Get_Chart_View_Should_Load_View_Data_Correctly_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;
            string type = "term";
            string value = termId.ToString();
            string major = majorId;
            string lecturerType = MyConstants.VisitingLecturerType;
            mockSetMajor.Setup(m => m.Find(It.IsAny<string>())).Returns(listMajor.First());

            // Act
            PartialViewResult result = controller.GetChart(isLesson, type, value, major, lecturerType) as PartialViewResult;
            dynamic viewData = result.ViewBag;
            bool isLessonViewdata = viewData.isLesson;
            string typeViewData = viewData.type;
            string valueViewData = viewData.value;
            string majorViewData = viewData.major;
            string lecturerTypeViewData = viewData.lecturerType;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(isLesson, isLessonViewdata);
            Assert.AreEqual(type, typeViewData);
            Assert.AreEqual(value, valueViewData);
            Assert.AreEqual(major, majorViewData);
            Assert.AreEqual(lecturerType, lecturerTypeViewData);
        }

        [TestMethod()]
        public void Get_Chart_View_Should_Load_View_Data_About_Major_Correctly_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;
            string type = "term";
            string value = termId.ToString();
            string major = majorId;
            string lecturerType = MyConstants.VisitingLecturerType;
            major testMajor = listMajor.First();
            mockSetMajor.Setup(m => m.Find(It.IsAny<string>())).Returns(testMajor);

            // Act
            PartialViewResult result = controller.GetChart(isLesson, type, value, major, lecturerType) as PartialViewResult;
            dynamic viewData = result.ViewBag;
            string majorAbbViewData = viewData.majorAbb;
            string majorNameViewData = viewData.majorName;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(testMajor.abbreviation, majorAbbViewData);
            Assert.AreEqual(testMajor.name, majorNameViewData);
        }

        [TestMethod()]
        public void Get_Chart_View_Should_Load_View_Data_About_All_Majors_Correctly_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;
            string type = "term";
            string value = termId.ToString();
            string major = "-1";
            string lecturerType = MyConstants.VisitingLecturerType;
            mockSetMajor.Setup(m => m.Find(It.IsAny<string>())).Returns(listMajor.First());

            // Act
            PartialViewResult result = controller.GetChart(isLesson, type, value, major, lecturerType) as PartialViewResult;
            dynamic viewData = result.ViewBag;
            string majorAbbViewData = viewData.majorAbb;
            string majorNameViewData = viewData.majorName;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("TatCa", majorAbbViewData);
            Assert.AreEqual("Tất cả", majorNameViewData);
        }

        [TestMethod()]
        public void Get_Term_And_Major_Statistics_Json_Data_Not_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, majorId, MyConstants.VisitingLecturerType);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            foreach (dynamic json in jsonCollection)
            {
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
        public void Get_Term_And_Major_Statistics_Json_Data_Not_False_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, majorId, MyConstants.VisitingLecturerType);

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            dynamic jsonCollection = actionResult.Data;
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.Key,
                    "JSON record does not contain \"Key\" required property.");
                Assert.IsNotNull(json.staff_id,
                    "JSON record does not contain \"staff_id\" required property.");
                Assert.IsNotNull(json.full_name,
                    "JSON record does not contain \"full_name\" required property.");
                Assert.IsNotNull(json.subject_count,
                    "JSON record does not contain \"subject_count\" required property.");
                Assert.IsNotNull(json.class_count,
                    "JSON record does not contain \"class_count\" required property.");
                Assert.IsNotNull(json.sum,
                    "JSON record does not contain \"sum\" required property.");
                Assert.IsNotNull(json.lecturer_type,
                    "JSON record does not contain \"lecturer_type\" required property.");
            }
        }

        [TestMethod()]
        public void Term_And_Major_Statistics_Json_Data_Should_Convert_To_IEnumerable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, majorId, MyConstants.VisitingLecturerType);
            dynamic jsonCollection = actionResult.Data;
            int count = 0;
            foreach (dynamic value in jsonCollection)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Term_And_Major_Statistics_Json_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, majorId, MyConstants.VisitingLecturerType);
            dynamic jsonCollection = actionResult.Data;

            // Assert                
            Assert.IsNotNull(jsonCollection[0]);
        }

        [TestMethod()]
        public void Term_And_Major_Statistics_JSon_Data_Should_Be_Indexable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, majorId, MyConstants.VisitingLecturerType);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            for (int i = 0; i < jsonCollection.Count; i++)
            {

                dynamic json = jsonCollection[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.Key,
                    "JSON record does not contain \"Key\" required property.");
                Assert.IsNotNull(json.staff_id,
                    "JSON record does not contain \"staff_id\" required property.");
                Assert.IsNotNull(json.full_name,
                    "JSON record does not contain \"full_name\" required property.");
                Assert.IsNotNull(json.subject_count,
                    "JSON record does not contain \"subject_count\" required property.");
                Assert.IsNotNull(json.class_count,
                    "JSON record does not contain \"class_count\" required property.");
                Assert.IsNotNull(json.sum,
                    "JSON record does not contain \"sum\" required property.");
                Assert.IsNotNull(json.lecturer_type,
                    "JSON record does not contain \"lecturer_type\" required property.");
            }
        }

        [TestMethod()]
        public void Get_Term_And_Major_Statistics_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, majorId, MyConstants.VisitingLecturerType);
            dynamic jsonCollection = actionResult.Data;
            IQueryable<IGrouping<string, class_section>> query_classSection = listClassSection.Where(c => c.term_id == termId && c.major_id == majorId && c.lecturer.type == MyConstants.VisitingLecturerType).GroupBy(c => c.lecturer_id);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(query_classSection.Count(), jsonCollection.Count);
        }

        [TestMethod()]
        public void Get_Term_And_Major_Statistics_Should_Order_By_Largest_To_Smallest_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;
            int previousSum = 0;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, majorId, MyConstants.VisitingLecturerType);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            foreach (dynamic json in jsonCollection)
            {
                int currentSum = json.sum;
                Assert.IsTrue(currentSum >= previousSum);
                previousSum = json.sum;
            }
        }

        [TestMethod()]
        public void Get_Term_Statistics_Json_Data_Not_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, "-1", MyConstants.VisitingLecturerType);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            foreach (dynamic json in jsonCollection)
            {
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
        public void Get_Term_Statistics_Json_Data_Not_False_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, "-1", MyConstants.VisitingLecturerType);

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            dynamic jsonCollection = actionResult.Data;
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.Key,
                    "JSON record does not contain \"Key\" required property.");
                Assert.IsNotNull(json.staff_id,
                    "JSON record does not contain \"staff_id\" required property.");
                Assert.IsNotNull(json.full_name,
                    "JSON record does not contain \"full_name\" required property.");
                Assert.IsNotNull(json.subject_count,
                    "JSON record does not contain \"subject_count\" required property.");
                Assert.IsNotNull(json.class_count,
                    "JSON record does not contain \"class_count\" required property.");
                Assert.IsNotNull(json.sum,
                    "JSON record does not contain \"sum\" required property.");
                Assert.IsNotNull(json.lecturer_type,
                    "JSON record does not contain \"lecturer_type\" required property.");
            }
        }

        [TestMethod()]
        public void Term_Statistics_Json_Data_Should_Convert_To_IEnumerable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, "-1", MyConstants.VisitingLecturerType);
            dynamic jsonCollection = actionResult.Data;
            int count = 0;
            foreach (dynamic value in jsonCollection)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Term_Statistics_Json_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, "-1", MyConstants.VisitingLecturerType);
            dynamic jsonCollection = actionResult.Data;

            // Assert                
            Assert.IsNotNull(jsonCollection[0]);
        }

        [TestMethod()]
        public void Term_Statistics_JSon_Data_Should_Be_Indexable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, "-1", MyConstants.VisitingLecturerType);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            for (int i = 0; i < jsonCollection.Count; i++)
            {

                dynamic json = jsonCollection[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.Key,
                    "JSON record does not contain \"Key\" required property.");
                Assert.IsNotNull(json.staff_id,
                    "JSON record does not contain \"staff_id\" required property.");
                Assert.IsNotNull(json.full_name,
                    "JSON record does not contain \"full_name\" required property.");
                Assert.IsNotNull(json.subject_count,
                    "JSON record does not contain \"subject_count\" required property.");
                Assert.IsNotNull(json.class_count,
                    "JSON record does not contain \"class_count\" required property.");
                Assert.IsNotNull(json.sum,
                    "JSON record does not contain \"sum\" required property.");
                Assert.IsNotNull(json.lecturer_type,
                    "JSON record does not contain \"lecturer_type\" required property.");
            }
        }

        [TestMethod()]
        public void Get_Term_Statistics_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, "-1", MyConstants.VisitingLecturerType);
            dynamic jsonCollection = actionResult.Data;
            IQueryable<IGrouping<string, class_section>> query_classSection = listClassSection.Where(c => c.term_id == termId && c.lecturer.type == MyConstants.VisitingLecturerType).GroupBy(c => c.lecturer_id);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(query_classSection.Count(), jsonCollection.Count);
        }

        [TestMethod()]
        public void Get_Term_Statistics_Should_Order_By_Largest_To_Smallest_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;
            int previousSum = 0;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, "-1", MyConstants.VisitingLecturerType);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            foreach (dynamic json in jsonCollection)
            {
                int currentSum = json.sum;
                Assert.IsTrue(currentSum >= previousSum);
                previousSum = json.sum;
            }
        }

        [TestMethod()]
        public void Get_Term_And_Major_Statistics_All_Json_Data_Not_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, majorId, "-1");
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            foreach (dynamic json in jsonCollection)
            {
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
        public void Get_Term_And_Major_Statistics_All_Json_Data_Not_False_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, majorId, "-1");

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            dynamic jsonCollection = actionResult.Data;
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.Key,
                    "JSON record does not contain \"Key\" required property.");
                Assert.IsNotNull(json.staff_id,
                    "JSON record does not contain \"staff_id\" required property.");
                Assert.IsNotNull(json.full_name,
                    "JSON record does not contain \"full_name\" required property.");
                Assert.IsNotNull(json.subject_count,
                    "JSON record does not contain \"subject_count\" required property.");
                Assert.IsNotNull(json.class_count,
                    "JSON record does not contain \"class_count\" required property.");
                Assert.IsNotNull(json.sum,
                    "JSON record does not contain \"sum\" required property.");
                Assert.IsNotNull(json.lecturer_type,
                    "JSON record does not contain \"lecturer_type\" required property.");
            }
        }

        [TestMethod()]
        public void Term_And_Major_Statistics_All_Json_Data_Should_Convert_To_IEnumerable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, majorId, "-1");
            dynamic jsonCollection = actionResult.Data;
            int count = 0;
            foreach (dynamic value in jsonCollection)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Term_And_Major_Statistics_All_Json_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, majorId, "-1");
            dynamic jsonCollection = actionResult.Data;

            // Assert                
            Assert.IsNotNull(jsonCollection[0]);
        }

        [TestMethod()]
        public void Term_And_Major_Statistics_All_JSon_Data_Should_Be_Indexable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, majorId, "-1");
            dynamic jsonCollection = actionResult.Data;

            // Assert
            for (int i = 0; i < jsonCollection.Count; i++)
            {

                dynamic json = jsonCollection[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.Key,
                    "JSON record does not contain \"Key\" required property.");
                Assert.IsNotNull(json.staff_id,
                    "JSON record does not contain \"staff_id\" required property.");
                Assert.IsNotNull(json.full_name,
                    "JSON record does not contain \"full_name\" required property.");
                Assert.IsNotNull(json.subject_count,
                    "JSON record does not contain \"subject_count\" required property.");
                Assert.IsNotNull(json.class_count,
                    "JSON record does not contain \"class_count\" required property.");
                Assert.IsNotNull(json.sum,
                    "JSON record does not contain \"sum\" required property.");
                Assert.IsNotNull(json.lecturer_type,
                    "JSON record does not contain \"lecturer_type\" required property.");
            }
        }

        [TestMethod()]
        public void Get_Term_And_Major_Statistics_All_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, majorId, "-1");
            dynamic jsonCollection = actionResult.Data;
            IQueryable<IGrouping<string, class_section>> query_classSection = listClassSection.Where(c => c.term_id == termId && c.major_id == majorId).GroupBy(c => c.lecturer_id);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(query_classSection.Count(), jsonCollection.Count);
        }

        [TestMethod()]
        public void Get_Term_And_Major_Statistics_All_Should_Order_By_Largest_To_Smallest_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;
            int previousSum = 0;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, majorId, "-1");
            dynamic jsonCollection = actionResult.Data;

            // Assert
            foreach (dynamic json in jsonCollection)
            {
                int currentSum = json.sum;
                Assert.IsTrue(currentSum >= previousSum);
                previousSum = json.sum;
            }
        }

        [TestMethod()]
        public void Get_Term_Statistics_All_Json_Data_Not_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, "-1", "-1");
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            foreach (dynamic json in jsonCollection)
            {
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
        public void Get_Term_Statistics_All_Json_Data_Not_False_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, "-1", "-1");

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            dynamic jsonCollection = actionResult.Data;
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.Key,
                    "JSON record does not contain \"Key\" required property.");
                Assert.IsNotNull(json.staff_id,
                    "JSON record does not contain \"staff_id\" required property.");
                Assert.IsNotNull(json.full_name,
                    "JSON record does not contain \"full_name\" required property.");
                Assert.IsNotNull(json.subject_count,
                    "JSON record does not contain \"subject_count\" required property.");
                Assert.IsNotNull(json.class_count,
                    "JSON record does not contain \"class_count\" required property.");
                Assert.IsNotNull(json.sum,
                    "JSON record does not contain \"sum\" required property.");
                Assert.IsNotNull(json.lecturer_type,
                    "JSON record does not contain \"lecturer_type\" required property.");
            }
        }

        [TestMethod()]
        public void Term_Statistics_All_Json_Data_Should_Convert_To_IEnumerable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, "-1", "-1");
            dynamic jsonCollection = actionResult.Data;
            int count = 0;
            foreach (dynamic value in jsonCollection)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Term_Statistics_All_Json_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, "-1", "-1");
            dynamic jsonCollection = actionResult.Data;

            // Assert                
            Assert.IsNotNull(jsonCollection[0]);
        }

        [TestMethod()]
        public void Term_Statistics_All_JSon_Data_Should_Be_Indexable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, "-1", "-1");
            dynamic jsonCollection = actionResult.Data;

            // Assert
            for (int i = 0; i < jsonCollection.Count; i++)
            {

                dynamic json = jsonCollection[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.Key,
                    "JSON record does not contain \"Key\" required property.");
                Assert.IsNotNull(json.staff_id,
                    "JSON record does not contain \"staff_id\" required property.");
                Assert.IsNotNull(json.full_name,
                    "JSON record does not contain \"full_name\" required property.");
                Assert.IsNotNull(json.subject_count,
                    "JSON record does not contain \"subject_count\" required property.");
                Assert.IsNotNull(json.class_count,
                    "JSON record does not contain \"class_count\" required property.");
                Assert.IsNotNull(json.sum,
                    "JSON record does not contain \"sum\" required property.");
                Assert.IsNotNull(json.lecturer_type,
                    "JSON record does not contain \"lecturer_type\" required property.");
            }
        }

        [TestMethod()]
        public void Get_Term_Statistics_All_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, "-1", "-1");
            dynamic jsonCollection = actionResult.Data;
            IQueryable<IGrouping<string, class_section>> query_classSection = listClassSection.Where(c => c.term_id == termId).GroupBy(c => c.lecturer_id);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(query_classSection.Count(), jsonCollection.Count);
        }

        [TestMethod()]
        public void Get_Term_Statistics_All_Should_Order_By_Largest_To_Smallest_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            bool isLesson = false;
            int previousSum = 0;

            // Act
            JsonResult actionResult = controller.GetTermData(isLesson, termId, "-1", "-1");
            dynamic jsonCollection = actionResult.Data;

            // Assert
            foreach (dynamic json in jsonCollection)
            {
                int currentSum = json.sum;
                Assert.IsTrue(currentSum >= previousSum);
                previousSum = json.sum;
            }
        }

        [TestMethod()]
        public void Get_Term_And_Major_Subjects_Json_Data_Not_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetTermSubjects(termId, majorId, userId1);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.id);
                Assert.IsNotNull(json.subject_name);
                Assert.IsNotNull(json.subject_credits);
                Assert.IsNotNull(json.subject_major);
                Assert.IsNotNull(json.subject_hours);
                Assert.IsNotNull(json.theory_count);
                Assert.IsNotNull(json.practice_count);
            }
        }


        [TestMethod()]
        public void Get_Term_And_Major_Subjects_Json_Data_Not_False_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetTermSubjects(termId, majorId, userId1);

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            dynamic jsonCollection = actionResult.Data;
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.id,
                    "JSON record does not contain \"id\" required property.");
                Assert.IsNotNull(json.subject_name,
                    "JSON record does not contain \"subject_name\" required property.");
                Assert.IsNotNull(json.subject_credits,
                    "JSON record does not contain \"subject_credits\" required property.");
                Assert.IsNotNull(json.subject_major,
                    "JSON record does not contain \"subject_major\" required property.");
                Assert.IsNotNull(json.subject_hours,
                    "JSON record does not contain \"subject_hours\" required property.");
                Assert.IsNotNull(json.theory_count,
                    "JSON record does not contain \"theory_count\" required property.");
                Assert.IsNotNull(json.practice_count,
                    "JSON record does not contain \"practice_count\" required property.");
            }
        }

        [TestMethod()]
        public void Term_And_Major_Subjects_Json_Data_Should_Convert_To_IEnumerable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetTermSubjects(termId, majorId, userId1);
            dynamic jsonCollection = actionResult.Data;
            int count = 0;
            foreach (dynamic value in jsonCollection)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Term_And_Major_Subjects_Json_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetTermSubjects(termId, majorId, userId1);
            dynamic jsonCollection = actionResult.Data;

            // Assert                
            Assert.IsNotNull(jsonCollection[0]);
        }

        [TestMethod()]
        public void Term_And_Major_Subjects_JSon_Data_Should_Be_Indexable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetTermSubjects(termId, majorId, userId1);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            for (int i = 0; i < jsonCollection.Count; i++)
            {

                dynamic json = jsonCollection[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.id,
                    "JSON record does not contain \"id\" required property.");
                Assert.IsNotNull(json.subject_name,
                    "JSON record does not contain \"subject_name\" required property.");
                Assert.IsNotNull(json.subject_credits,
                    "JSON record does not contain \"subject_credits\" required property.");
                Assert.IsNotNull(json.subject_major,
                    "JSON record does not contain \"subject_major\" required property.");
                Assert.IsNotNull(json.subject_hours,
                    "JSON record does not contain \"subject_hours\" required property.");
                Assert.IsNotNull(json.theory_count,
                    "JSON record does not contain \"theory_count\" required property.");
                Assert.IsNotNull(json.practice_count,
                    "JSON record does not contain \"practice_count\" required property.");
            }
        }

        [TestMethod()]
        public void Get_Term_And_Major_Subjects_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetTermSubjects(termId, majorId, userId1);
            dynamic jsonCollection = actionResult.Data;
            IQueryable<IGrouping<string, class_section>> query_classSection = listClassSection.Where(c => c.term_id == termId && c.major_id == majorId && c.lecturer_id == userId1).GroupBy(c => c.subject_id);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(query_classSection.Count(), jsonCollection.Count);
        }

        [TestMethod()]
        public void Get_Term_Subjects_Json_Data_Not_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetTermSubjects(termId, "-1", userId1);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.id);
                Assert.IsNotNull(json.subject_name);
                Assert.IsNotNull(json.subject_credits);
                Assert.IsNotNull(json.subject_major);
                Assert.IsNotNull(json.subject_hours);
                Assert.IsNotNull(json.theory_count);
                Assert.IsNotNull(json.practice_count);
            }
        }


        [TestMethod()]
        public void Get_Term_Subjects_Json_Data_Not_False_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetTermSubjects(termId, "-1", userId1);

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            dynamic jsonCollection = actionResult.Data;
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.id,
                    "JSON record does not contain \"id\" required property.");
                Assert.IsNotNull(json.subject_name,
                    "JSON record does not contain \"subject_name\" required property.");
                Assert.IsNotNull(json.subject_credits,
                    "JSON record does not contain \"subject_credits\" required property.");
                Assert.IsNotNull(json.subject_major,
                    "JSON record does not contain \"subject_major\" required property.");
                Assert.IsNotNull(json.subject_hours,
                    "JSON record does not contain \"subject_hours\" required property.");
                Assert.IsNotNull(json.theory_count,
                    "JSON record does not contain \"theory_count\" required property.");
                Assert.IsNotNull(json.practice_count,
                    "JSON record does not contain \"practice_count\" required property.");
            }
        }

        [TestMethod()]
        public void Term_Subjects_Json_Data_Should_Convert_To_IEnumerable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetTermSubjects(termId, "-1", userId1);
            dynamic jsonCollection = actionResult.Data;
            int count = 0;
            foreach (dynamic value in jsonCollection)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Term_Subjects_Json_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetTermSubjects(termId, "-1", userId1);
            dynamic jsonCollection = actionResult.Data;

            // Assert                
            Assert.IsNotNull(jsonCollection[0]);
        }

        [TestMethod()]
        public void Term_Subjects_JSon_Data_Should_Be_Indexable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetTermSubjects(termId, "-1", userId1);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            for (int i = 0; i < jsonCollection.Count; i++)
            {

                dynamic json = jsonCollection[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.id,
                    "JSON record does not contain \"id\" required property.");
                Assert.IsNotNull(json.subject_name,
                    "JSON record does not contain \"subject_name\" required property.");
                Assert.IsNotNull(json.subject_credits,
                    "JSON record does not contain \"subject_credits\" required property.");
                Assert.IsNotNull(json.subject_major,
                    "JSON record does not contain \"subject_major\" required property.");
                Assert.IsNotNull(json.subject_hours,
                    "JSON record does not contain \"subject_hours\" required property.");
                Assert.IsNotNull(json.theory_count,
                    "JSON record does not contain \"theory_count\" required property.");
                Assert.IsNotNull(json.practice_count,
                    "JSON record does not contain \"practice_count\" required property.");
            }
        }

        [TestMethod()]
        public void Get_Term_Subjects_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetTermSubjects(termId, "-1", userId1);
            dynamic jsonCollection = actionResult.Data;
            IQueryable<IGrouping<string, class_section>> query_classSection = listClassSection.Where(c => c.term_id == termId && c.lecturer_id == userId1).GroupBy(c => c.subject_id);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(query_classSection.Count(), jsonCollection.Count);
        }

        [TestMethod()]
        public void Get_Year_And_Major_Statistics_Json_Data_Not_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, majorId, MyConstants.VisitingLecturerType);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            foreach (dynamic json in jsonCollection)
            {
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
        public void Get_Year_And_Major_Statistics_Json_Data_Not_False_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, majorId, MyConstants.VisitingLecturerType);

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            dynamic jsonCollection = actionResult.Data;
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.Key,
                    "JSON record does not contain \"Key\" required property.");
                Assert.IsNotNull(json.staff_id,
                    "JSON record does not contain \"staff_id\" required property.");
                Assert.IsNotNull(json.full_name,
                    "JSON record does not contain \"full_name\" required property.");
                Assert.IsNotNull(json.subject_count,
                    "JSON record does not contain \"subject_count\" required property.");
                Assert.IsNotNull(json.class_count,
                    "JSON record does not contain \"class_count\" required property.");
                Assert.IsNotNull(json.sum,
                    "JSON record does not contain \"sum\" required property.");
                Assert.IsNotNull(json.lecturer_type,
                    "JSON record does not contain \"lecturer_type\" required property.");
            }
        }

        [TestMethod()]
        public void Year_And_Major_Statistics_Json_Data_Should_Convert_To_IEnumerable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, majorId, MyConstants.VisitingLecturerType);
            dynamic jsonCollection = actionResult.Data;
            int count = 0;
            foreach (dynamic value in jsonCollection)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Year_And_Major_Statistics_Json_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, majorId, MyConstants.VisitingLecturerType);
            dynamic jsonCollection = actionResult.Data;

            // Assert                
            Assert.IsNotNull(jsonCollection[0]);
        }

        [TestMethod()]
        public void Year_And_Major_Statistics_JSon_Data_Should_Be_Indexable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, majorId, MyConstants.VisitingLecturerType);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            for (int i = 0; i < jsonCollection.Count; i++)
            {

                dynamic json = jsonCollection[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.Key,
                    "JSON record does not contain \"Key\" required property.");
                Assert.IsNotNull(json.staff_id,
                    "JSON record does not contain \"staff_id\" required property.");
                Assert.IsNotNull(json.full_name,
                    "JSON record does not contain \"full_name\" required property.");
                Assert.IsNotNull(json.subject_count,
                    "JSON record does not contain \"subject_count\" required property.");
                Assert.IsNotNull(json.class_count,
                    "JSON record does not contain \"class_count\" required property.");
                Assert.IsNotNull(json.sum,
                    "JSON record does not contain \"sum\" required property.");
                Assert.IsNotNull(json.lecturer_type,
                    "JSON record does not contain \"lecturer_type\" required property.");
            }
        }

        [TestMethod()]
        public void Get_Year_And_Major_Statistics_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, majorId, MyConstants.VisitingLecturerType);
            dynamic jsonCollection = actionResult.Data;
            IQueryable<IGrouping<string, class_section>> query_classSection = listClassSection.Where(c => c.term_id == termId && c.major_id == majorId && c.lecturer.type == MyConstants.VisitingLecturerType).GroupBy(c => c.lecturer_id);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(query_classSection.Count(), jsonCollection.Count);
        }

        [TestMethod()]
        public void Get_Year_And_Major_Statistics_Should_Order_By_Largest_To_Smallest_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;
            int previousSum = 0;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, majorId, MyConstants.VisitingLecturerType);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            foreach (dynamic json in jsonCollection)
            {
                int currentSum = json.sum;
                Assert.IsTrue(currentSum >= previousSum);
                previousSum = json.sum;
            }
        }

        [TestMethod()]
        public void Get_Year_Statistics_Json_Data_Not_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, "-1", MyConstants.VisitingLecturerType);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            foreach (dynamic json in jsonCollection)
            {
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
        public void Get_Year_Statistics_Json_Data_Not_False_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, "-1", MyConstants.VisitingLecturerType);

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            dynamic jsonCollection = actionResult.Data;
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.Key,
                    "JSON record does not contain \"Key\" required property.");
                Assert.IsNotNull(json.staff_id,
                    "JSON record does not contain \"staff_id\" required property.");
                Assert.IsNotNull(json.full_name,
                    "JSON record does not contain \"full_name\" required property.");
                Assert.IsNotNull(json.subject_count,
                    "JSON record does not contain \"subject_count\" required property.");
                Assert.IsNotNull(json.class_count,
                    "JSON record does not contain \"class_count\" required property.");
                Assert.IsNotNull(json.sum,
                    "JSON record does not contain \"sum\" required property.");
                Assert.IsNotNull(json.lecturer_type,
                    "JSON record does not contain \"lecturer_type\" required property.");
            }
        }

        [TestMethod()]
        public void Year_Statistics_Json_Data_Should_Convert_To_IEnumerable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, "-1", MyConstants.VisitingLecturerType);
            dynamic jsonCollection = actionResult.Data;
            int count = 0;
            foreach (dynamic value in jsonCollection)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Year_Statistics_Json_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, "-1", MyConstants.VisitingLecturerType);
            dynamic jsonCollection = actionResult.Data;

            // Assert                
            Assert.IsNotNull(jsonCollection[0]);
        }

        [TestMethod()]
        public void Year_Statistics_JSon_Data_Should_Be_Indexable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, "-1", MyConstants.VisitingLecturerType);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            for (int i = 0; i < jsonCollection.Count; i++)
            {

                dynamic json = jsonCollection[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.Key,
                    "JSON record does not contain \"Key\" required property.");
                Assert.IsNotNull(json.staff_id,
                    "JSON record does not contain \"staff_id\" required property.");
                Assert.IsNotNull(json.full_name,
                    "JSON record does not contain \"full_name\" required property.");
                Assert.IsNotNull(json.subject_count,
                    "JSON record does not contain \"subject_count\" required property.");
                Assert.IsNotNull(json.class_count,
                    "JSON record does not contain \"class_count\" required property.");
                Assert.IsNotNull(json.sum,
                    "JSON record does not contain \"sum\" required property.");
                Assert.IsNotNull(json.lecturer_type,
                    "JSON record does not contain \"lecturer_type\" required property.");
            }
        }

        [TestMethod()]
        public void Get_Year_Statistics_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, "-1", MyConstants.VisitingLecturerType);
            dynamic jsonCollection = actionResult.Data;
            IQueryable<IGrouping<string, class_section>> query_classSection = listClassSection.Where(c => c.term_id == termId && c.lecturer.type == MyConstants.VisitingLecturerType).GroupBy(c => c.lecturer_id);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(query_classSection.Count(), jsonCollection.Count);
        }

        [TestMethod()]
        public void Get_Year_Statistics_Should_Order_By_Largest_To_Smallest_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;
            int previousSum = 0;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, "-1", MyConstants.VisitingLecturerType);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            foreach (dynamic json in jsonCollection)
            {
                int currentSum = json.sum;
                Assert.IsTrue(currentSum >= previousSum);
                previousSum = json.sum;
            }
        }

        [TestMethod()]
        public void Get_Year_And_Major_Statistics_All_Json_Data_Not_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, majorId, "-1");
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            foreach (dynamic json in jsonCollection)
            {
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
        public void Get_Year_And_Major_Statistics_All_Json_Data_Not_False_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, majorId, "-1");

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            dynamic jsonCollection = actionResult.Data;
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.Key,
                    "JSON record does not contain \"Key\" required property.");
                Assert.IsNotNull(json.staff_id,
                    "JSON record does not contain \"staff_id\" required property.");
                Assert.IsNotNull(json.full_name,
                    "JSON record does not contain \"full_name\" required property.");
                Assert.IsNotNull(json.subject_count,
                    "JSON record does not contain \"subject_count\" required property.");
                Assert.IsNotNull(json.class_count,
                    "JSON record does not contain \"class_count\" required property.");
                Assert.IsNotNull(json.sum,
                    "JSON record does not contain \"sum\" required property.");
                Assert.IsNotNull(json.lecturer_type,
                    "JSON record does not contain \"lecturer_type\" required property.");
            }
        }

        [TestMethod()]
        public void Year_And_Major_Statistics_All_Json_Data_Should_Convert_To_IEnumerable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, majorId, "-1");
            dynamic jsonCollection = actionResult.Data;
            int count = 0;
            foreach (dynamic value in jsonCollection)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Year_And_Major_Statistics_All_Json_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, majorId, "-1");
            dynamic jsonCollection = actionResult.Data;

            // Assert                
            Assert.IsNotNull(jsonCollection[0]);
        }

        [TestMethod()]
        public void Year_And_Major_Statistics_All_JSon_Data_Should_Be_Indexable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, majorId, "-1");
            dynamic jsonCollection = actionResult.Data;

            // Assert
            for (int i = 0; i < jsonCollection.Count; i++)
            {

                dynamic json = jsonCollection[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.Key,
                    "JSON record does not contain \"Key\" required property.");
                Assert.IsNotNull(json.staff_id,
                    "JSON record does not contain \"staff_id\" required property.");
                Assert.IsNotNull(json.full_name,
                    "JSON record does not contain \"full_name\" required property.");
                Assert.IsNotNull(json.subject_count,
                    "JSON record does not contain \"subject_count\" required property.");
                Assert.IsNotNull(json.class_count,
                    "JSON record does not contain \"class_count\" required property.");
                Assert.IsNotNull(json.sum,
                    "JSON record does not contain \"sum\" required property.");
                Assert.IsNotNull(json.lecturer_type,
                    "JSON record does not contain \"lecturer_type\" required property.");
            }
        }

        [TestMethod()]
        public void Get_Year_And_Major_Statistics_All_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, majorId, "-1");
            dynamic jsonCollection = actionResult.Data;
            IQueryable<IGrouping<string, class_section>> query_classSection = listClassSection.Where(c => c.term.start_year == startYear && c.term.end_year == endYear && c.major_id == majorId).GroupBy(c => c.lecturer_id);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(query_classSection.Count(), jsonCollection.Count);
        }

        [TestMethod()]
        public void Get_Year_And_Major_Statistics_All_Should_Order_By_Largest_To_Smallest_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;
            int previousSum = 0;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, majorId, "-1");
            dynamic jsonCollection = actionResult.Data;

            // Assert
            foreach (dynamic json in jsonCollection)
            {
                int currentSum = json.sum;
                Assert.IsTrue(currentSum >= previousSum);
                previousSum = json.sum;
            }
        }

        [TestMethod()]
        public void Get_Year_Statistics_All_Json_Data_Not_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, "-1", "-1");
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            foreach (dynamic json in jsonCollection)
            {
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
        public void Get_Year_Statistics_All_Json_Data_Not_False_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, "-1", "-1");

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            dynamic jsonCollection = actionResult.Data;
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.Key,
                    "JSON record does not contain \"Key\" required property.");
                Assert.IsNotNull(json.staff_id,
                    "JSON record does not contain \"staff_id\" required property.");
                Assert.IsNotNull(json.full_name,
                    "JSON record does not contain \"full_name\" required property.");
                Assert.IsNotNull(json.subject_count,
                    "JSON record does not contain \"subject_count\" required property.");
                Assert.IsNotNull(json.class_count,
                    "JSON record does not contain \"class_count\" required property.");
                Assert.IsNotNull(json.sum,
                    "JSON record does not contain \"sum\" required property.");
                Assert.IsNotNull(json.lecturer_type,
                    "JSON record does not contain \"lecturer_type\" required property.");
            }
        }

        [TestMethod()]
        public void Year_Statistics_All_Json_Data_Should_Convert_To_IEnumerable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, "-1", "-1");
            dynamic jsonCollection = actionResult.Data;
            int count = 0;
            foreach (dynamic value in jsonCollection)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Year_Statistics_All_Json_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, "-1", "-1");
            dynamic jsonCollection = actionResult.Data;

            // Assert                
            Assert.IsNotNull(jsonCollection[0]);
        }

        [TestMethod()]
        public void Year_Statistics_All_JSon_Data_Should_Be_Indexable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, "-1", "-1");
            dynamic jsonCollection = actionResult.Data;

            // Assert
            for (int i = 0; i < jsonCollection.Count; i++)
            {

                dynamic json = jsonCollection[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.Key,
                    "JSON record does not contain \"Key\" required property.");
                Assert.IsNotNull(json.staff_id,
                    "JSON record does not contain \"staff_id\" required property.");
                Assert.IsNotNull(json.full_name,
                    "JSON record does not contain \"full_name\" required property.");
                Assert.IsNotNull(json.subject_count,
                    "JSON record does not contain \"subject_count\" required property.");
                Assert.IsNotNull(json.class_count,
                    "JSON record does not contain \"class_count\" required property.");
                Assert.IsNotNull(json.sum,
                    "JSON record does not contain \"sum\" required property.");
                Assert.IsNotNull(json.lecturer_type,
                    "JSON record does not contain \"lecturer_type\" required property.");
            }
        }

        [TestMethod()]
        public void Get_Year_Statistics_All_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, "-1", "-1");
            dynamic jsonCollection = actionResult.Data;
            IQueryable<IGrouping<string, class_section>> query_classSection = listClassSection.Where(c => c.term.start_year == startYear && c.term.end_year == endYear && c.major_id == majorId).GroupBy(c => c.lecturer_id);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(query_classSection.Count(), jsonCollection.Count);
        }

        [TestMethod()]
        public void Get_Year_Statistics_All_Should_Order_By_Largest_To_Smallest_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;
            bool isLesson = false;
            int previousSum = 0;

            // Act
            JsonResult actionResult = controller.GetYearData(isLesson, startYear, endYear, "-1", "-1");
            dynamic jsonCollection = actionResult.Data;

            // Assert
            foreach (dynamic json in jsonCollection)
            {
                int currentSum = json.sum;
                Assert.IsTrue(currentSum >= previousSum);
                previousSum = json.sum;
            }
        }

        [TestMethod()]
        public void Get_Year_And_Major_Subjects_Json_Data_Not_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;

            // Act
            JsonResult actionResult = controller.GetYearsubjects(startYear, endYear, majorId, userId1);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.id);
                Assert.IsNotNull(json.subject_name);
                Assert.IsNotNull(json.subject_credits);
                Assert.IsNotNull(json.subject_major);
                Assert.IsNotNull(json.subject_hours);
                Assert.IsNotNull(json.theory_count);
                Assert.IsNotNull(json.practice_count);
            }
        }


        [TestMethod()]
        public void Get_Year_And_Major_Subjects_Json_Data_Not_False_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;

            // Act
            JsonResult actionResult = controller.GetYearsubjects(startYear, endYear, majorId, userId1);

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            dynamic jsonCollection = actionResult.Data;
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.id,
                    "JSON record does not contain \"id\" required property.");
                Assert.IsNotNull(json.subject_name,
                    "JSON record does not contain \"subject_name\" required property.");
                Assert.IsNotNull(json.subject_credits,
                    "JSON record does not contain \"subject_credits\" required property.");
                Assert.IsNotNull(json.subject_major,
                    "JSON record does not contain \"subject_major\" required property.");
                Assert.IsNotNull(json.subject_hours,
                    "JSON record does not contain \"subject_hours\" required property.");
                Assert.IsNotNull(json.theory_count,
                    "JSON record does not contain \"theory_count\" required property.");
                Assert.IsNotNull(json.practice_count,
                    "JSON record does not contain \"practice_count\" required property.");
            }
        }

        [TestMethod()]
        public void Year_And_Major_Subjects_Json_Data_Should_Convert_To_IEnumerable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;

            // Act
            JsonResult actionResult = controller.GetYearsubjects(startYear, endYear, majorId, userId1);
            dynamic jsonCollection = actionResult.Data;
            int count = 0;
            foreach (dynamic value in jsonCollection)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Year_And_Major_Subjects_Json_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;

            // Act
            JsonResult actionResult = controller.GetYearsubjects(startYear, endYear, majorId, userId1);
            dynamic jsonCollection = actionResult.Data;

            // Assert                
            Assert.IsNotNull(jsonCollection[0]);
        }

        [TestMethod()]
        public void Year_And_Major_Subjects_JSon_Data_Should_Be_Indexable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;

            // Act
            JsonResult actionResult = controller.GetYearsubjects(startYear, endYear, majorId, userId1);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            for (int i = 0; i < jsonCollection.Count; i++)
            {

                dynamic json = jsonCollection[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.id,
                    "JSON record does not contain \"id\" required property.");
                Assert.IsNotNull(json.subject_name,
                    "JSON record does not contain \"subject_name\" required property.");
                Assert.IsNotNull(json.subject_credits,
                    "JSON record does not contain \"subject_credits\" required property.");
                Assert.IsNotNull(json.subject_major,
                    "JSON record does not contain \"subject_major\" required property.");
                Assert.IsNotNull(json.subject_hours,
                    "JSON record does not contain \"subject_hours\" required property.");
                Assert.IsNotNull(json.theory_count,
                    "JSON record does not contain \"theory_count\" required property.");
                Assert.IsNotNull(json.practice_count,
                    "JSON record does not contain \"practice_count\" required property.");
            }
        }

        [TestMethod()]
        public void Get_Year_And_Major_Subjects_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;

            // Act
            JsonResult actionResult = controller.GetYearsubjects(startYear, endYear, majorId, userId1);
            dynamic jsonCollection = actionResult.Data;
            IQueryable<IGrouping<string, class_section>> query_classSection = listClassSection.Where(c => c.term.start_year == startYear && c.term.end_year == endYear && c.major_id == majorId && c.lecturer_id == userId1).GroupBy(c => c.subject_id);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(query_classSection.Count(), jsonCollection.Count);
        }

        [TestMethod()]
        public void Get_Year_Subjects_Json_Data_Not_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;

            // Act
            JsonResult actionResult = controller.GetYearsubjects(startYear, endYear, "-1", userId1);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.id);
                Assert.IsNotNull(json.subject_name);
                Assert.IsNotNull(json.subject_credits);
                Assert.IsNotNull(json.subject_major);
                Assert.IsNotNull(json.subject_hours);
                Assert.IsNotNull(json.theory_count);
                Assert.IsNotNull(json.practice_count);
            }
        }


        [TestMethod()]
        public void Get_Year_Subjects_Json_Data_Not_False_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;

            // Act
            JsonResult actionResult = controller.GetYearsubjects(startYear, endYear, "-1", userId1);

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            dynamic jsonCollection = actionResult.Data;
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.id,
                    "JSON record does not contain \"id\" required property.");
                Assert.IsNotNull(json.subject_name,
                    "JSON record does not contain \"subject_name\" required property.");
                Assert.IsNotNull(json.subject_credits,
                    "JSON record does not contain \"subject_credits\" required property.");
                Assert.IsNotNull(json.subject_major,
                    "JSON record does not contain \"subject_major\" required property.");
                Assert.IsNotNull(json.subject_hours,
                    "JSON record does not contain \"subject_hours\" required property.");
                Assert.IsNotNull(json.theory_count,
                    "JSON record does not contain \"theory_count\" required property.");
                Assert.IsNotNull(json.practice_count,
                    "JSON record does not contain \"practice_count\" required property.");
            }
        }

        [TestMethod()]
        public void Year_Subjects_Json_Data_Should_Convert_To_IEnumerable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;

            // Act
            JsonResult actionResult = controller.GetYearsubjects(startYear, endYear, "-1", userId1);
            dynamic jsonCollection = actionResult.Data;
            int count = 0;
            foreach (dynamic value in jsonCollection)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Year_Subjects_Json_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;

            // Act
            JsonResult actionResult = controller.GetYearsubjects(startYear, endYear, "-1", userId1);
            dynamic jsonCollection = actionResult.Data;

            // Assert                
            Assert.IsNotNull(jsonCollection[0]);
        }

        [TestMethod()]
        public void Year_Subjects_JSon_Data_Should_Be_Indexable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;

            // Act
            JsonResult actionResult = controller.GetYearsubjects(startYear, endYear, "-1", userId1);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            for (int i = 0; i < jsonCollection.Count; i++)
            {

                dynamic json = jsonCollection[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.id,
                    "JSON record does not contain \"id\" required property.");
                Assert.IsNotNull(json.subject_name,
                    "JSON record does not contain \"subject_name\" required property.");
                Assert.IsNotNull(json.subject_credits,
                    "JSON record does not contain \"subject_credits\" required property.");
                Assert.IsNotNull(json.subject_major,
                    "JSON record does not contain \"subject_major\" required property.");
                Assert.IsNotNull(json.subject_hours,
                    "JSON record does not contain \"subject_hours\" required property.");
                Assert.IsNotNull(json.theory_count,
                    "JSON record does not contain \"theory_count\" required property.");
                Assert.IsNotNull(json.practice_count,
                    "JSON record does not contain \"practice_count\" required property.");
            }
        }

        [TestMethod()]
        public void Get_Year_Subjects_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            term term = listTerm.First();
            int startYear = term.start_year;
            int endYear = term.end_year;

            // Act
            JsonResult actionResult = controller.GetYearsubjects(startYear, endYear, "-1", userId1);
            dynamic jsonCollection = actionResult.Data;
            IQueryable<IGrouping<string, class_section>> query_classSection = listClassSection.Where(c => c.term.start_year == startYear && c.term.end_year == endYear && c.lecturer_id == userId1).GroupBy(c => c.subject_id);

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(query_classSection.Count(), jsonCollection.Count);
        }

        [TestMethod()]
        public void Timetable_View_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController();

            // Act
            ViewResult result = controller.Timetable() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void Timetable_View_Should_Load_Term_SelectList_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);

            // Act
            ViewResult result = controller.Timetable() as ViewResult;
            SelectList termList = new SelectList(listTerm);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(termList.Count(), ((IEnumerable<dynamic>)result.ViewBag.term).Count());
        }

        [TestMethod()]
        public void Timetable_View_Should_Load_Term_SelectList_Data_Correctly_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);

            // Act
            ViewResult result = controller.Timetable() as ViewResult;
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
        public void Get_Timetable_View_Not_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            int week = listClassSection.First().start_week;
            Mock<HttpRequestBase> request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.UserLanguages).Returns(new string[] { "en" });
            Mock<HttpContextBase> context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(listTerm.First());

            // Act
            PartialViewResult result = controller.GetTimetable(termId, week) as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Get_Timetable_View_Name_Should_Be_Correct_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            int week = listClassSection.First().start_week;
            Mock<HttpRequestBase> request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.UserLanguages).Returns(new string[] { "en" });
            Mock<HttpContextBase> context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(listTerm.First());

            // Act
            PartialViewResult result = controller.GetTimetable(termId, week) as PartialViewResult;

            // Assert
            Assert.AreEqual("_Timetable", result.ViewName);
        }

        [TestMethod]
        public void Get_Timetable_View_Model_Should_Not_Be_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            int week = listClassSection.First().start_week;
            Mock<HttpRequestBase> request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.UserLanguages).Returns(new string[] { "en" });
            Mock<HttpContextBase> context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(listTerm.First());

            // Act
            PartialViewResult result = controller.GetTimetable(termId, week) as PartialViewResult;
            TimetableViewModel viewModel = (TimetableViewModel)result.ViewData.Model;

            // Assert
            Assert.IsNotNull(viewModel);
        }

        [TestMethod]
        public void Get_Timetable_View_Model_LecturerDTOs_Should_Not_Be_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            int week = listClassSection.First().start_week;
            Mock<HttpRequestBase> request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.UserLanguages).Returns(new string[] { "en" });
            Mock<HttpContextBase> context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(listTerm.First());

            // Act
            PartialViewResult result = controller.GetTimetable(termId, week) as PartialViewResult;
            TimetableViewModel viewModel = (TimetableViewModel)result.ViewData.Model;

            // Assert
            Assert.IsNotNull(viewModel);
            Assert.IsNotNull(viewModel.LecturerDTOs);
        }

        [TestMethod()]
        public void Get_Timetable_View_Model_LecturerDTOs_Data_Is_Correct_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            int week = listClassSection.First().start_week;
            Mock<HttpRequestBase> request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.UserLanguages).Returns(new string[] { "en" });
            Mock<HttpContextBase> context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(listTerm.First());
            List<lecturer> lecturerList = listLecturer.ToList();

            // Act
            PartialViewResult result = controller.GetTimetable(termId, week) as PartialViewResult;
            TimetableViewModel viewModel = (TimetableViewModel)result.ViewData.Model;
            List<LecturerDTO> lecturerDTOs = viewModel.LecturerDTOs.ToList();

            // Assert
            Assert.IsNotNull(lecturerDTOs);
            for (int i = 0; i < listLecturer.Count(); i++)
            {
                Assert.AreEqual(lecturerDTOs[i].Id, lecturerList[i].id);
                Assert.AreEqual(lecturerDTOs[i].FullName, lecturerList[i].full_name);
                Assert.AreEqual(lecturerDTOs[i].Type, lecturerList[i].type);
            }
        }

        [TestMethod()]
        public void Get_Timetable_View_Model_LecturerDTOs_Data_Should_Be_IEnumerable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            int week = listClassSection.First().start_week;
            Mock<HttpRequestBase> request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.UserLanguages).Returns(new string[] { "en" });
            Mock<HttpContextBase> context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(listTerm.First());
            List<lecturer> lecturerList = listLecturer.ToList();

            // Act
            PartialViewResult result = controller.GetTimetable(termId, week) as PartialViewResult;
            TimetableViewModel viewModel = (TimetableViewModel)result.ViewData.Model;
            IEnumerable<LecturerDTO> lecturerDTOs = viewModel.LecturerDTOs;
            int count = 0;
            foreach (dynamic value in lecturerDTOs)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Get_Timetable_View_Model_LecturerDTOs_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            int week = listClassSection.First().start_week;
            Mock<HttpRequestBase> request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.UserLanguages).Returns(new string[] { "en" });
            Mock<HttpContextBase> context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(listTerm.First());
            List<lecturer> lecturerList = listLecturer.ToList();

            // Act
            PartialViewResult result = controller.GetTimetable(termId, week) as PartialViewResult;
            TimetableViewModel viewModel = (TimetableViewModel)result.ViewData.Model;
            List<LecturerDTO> lecturerDTOs = viewModel.LecturerDTOs.ToList();

            // Assert
            Assert.IsNotNull(lecturerDTOs[0]);
        }

        [TestMethod()]
        public void Get_Timetable_View_Model_LecturerDTOs_Data_Should_Be_Indexable_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            int week = listClassSection.First().start_week;
            Mock<HttpRequestBase> request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.UserLanguages).Returns(new string[] { "en" });
            Mock<HttpContextBase> context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(listTerm.First());
            List<lecturer> lecturerList = listLecturer.ToList();

            // Act
            PartialViewResult result = controller.GetTimetable(termId, week) as PartialViewResult;
            TimetableViewModel viewModel = (TimetableViewModel)result.ViewData.Model;
            List<LecturerDTO> lecturerDTOs = viewModel.LecturerDTOs.ToList();

            // Assert
            for (int i = 0; i < lecturerDTOs.Count; i++)
            {

                dynamic json = lecturerDTOs[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.Id);
                Assert.IsNotNull(json.FullName);
                Assert.IsNotNull(json.Type);
            }
        }

        [TestMethod()]
        public void Get_Timetable_View_Model_LecturerDTOs_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Arrange
            StatisticsController controller = new StatisticsController(unitOfWork);
            int week = listClassSection.First().start_week;
            Mock<HttpRequestBase> request = new Mock<HttpRequestBase>();
            request.SetupGet(x => x.UserLanguages).Returns(new string[] { "en" });
            Mock<HttpContextBase> context = new Mock<HttpContextBase>();
            context.SetupGet(x => x.Request).Returns(request.Object);
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(listTerm.First());
            List<lecturer> lecturerList = listLecturer.ToList();

            // Act
            PartialViewResult result = controller.GetTimetable(termId, week) as PartialViewResult;
            TimetableViewModel viewModel = (TimetableViewModel)result.ViewData.Model;
            List<LecturerDTO> lecturerDTOs = viewModel.LecturerDTOs.ToList();

            // Assert
            Assert.IsNotNull(lecturerDTOs);
            Assert.AreEqual(listLecturer.Count(), lecturerDTOs.Count());
        }
    }
}