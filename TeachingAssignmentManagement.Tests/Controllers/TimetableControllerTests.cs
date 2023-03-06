using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Helpers;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Controllers.Tests
{
    [TestClass()]
    public class TimetableControllerTests
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

        public TimetableControllerTests()
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

        [TestMethod()]
        public void Get_Data_In_Term_Major_Return_Correct_Partial_View_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);

            // Act
            PartialViewResult result = controller.GetData(termId, majorId) as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("_Timetable", result.ViewName);
        }

        [TestMethod()]
        public void Get_Data_Partial_View_Should_Load_Class_In_Term_Major_Not_Null_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);

            // Act
            PartialViewResult result = controller.GetData(termId, majorId) as PartialViewResult;
            object modelResult = result.Model;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(modelResult);
        }

        [TestMethod()]
        public void Get_Data_Partial_View_Should_Load_Class_Section_View_Models_In_Term_Major_Not_Null_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);

            // Act
            PartialViewResult result = controller.GetData(termId, majorId) as PartialViewResult;
            TimetableViewModel modelResult = result.Model as TimetableViewModel;
            IEnumerable<ClassSectionDTO> classSectionDTOs = modelResult.ClassSectionDTOs;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(classSectionDTOs);
        }

        [TestMethod()]
        public void Get_Data_Partial_View_Should_Load_Class_Section_View_Models_In_Term_Major_Equal_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);

            // Act
            PartialViewResult result = controller.GetData(termId, majorId) as PartialViewResult;
            TimetableViewModel modelResult = result.Model as TimetableViewModel;
            IEnumerable<ClassSectionDTO> classSectionDTOs = modelResult.ClassSectionDTOs;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(listClassSection.Count(), classSectionDTOs.Count());
        }

        [TestMethod()]
        public void Get_Data_Partial_View_Should_Load_Class_Section_View_Models_In_Term_Major_Data_Correctly_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);

            // Act
            PartialViewResult result = controller.GetData(termId, majorId) as PartialViewResult;
            TimetableViewModel modelResult = result.Model as TimetableViewModel;
            List<ClassSectionDTO> classSectionDTOs = modelResult.ClassSectionDTOs.ToList();
            List<class_section> classSectionList = listClassSection.ToList();

            // Assert
            Assert.IsNotNull(result);
            for (int i = 0; i < classSectionList.Count(); i++)
            {
                Assert.AreEqual(classSectionDTOs[i].Id, classSectionList[i].id);
                Assert.AreEqual(classSectionDTOs[i].ClassSectionId, classSectionList[i].class_section_id);
                Assert.AreEqual(classSectionDTOs[i].Type, classSectionList[i].type);
                Assert.AreEqual(classSectionDTOs[i].Day2, classSectionList[i].day_2);
                Assert.AreEqual(classSectionDTOs[i].StartLesson2, classSectionList[i].start_lesson_2);
                Assert.AreEqual(classSectionDTOs[i].StudentRegisteredNumber, classSectionList[i].student_registered_number);
                Assert.AreEqual(classSectionDTOs[i].LastAssigned, classSectionList[i].lecturer1.full_name);
                Assert.AreEqual(classSectionDTOs[i].LecturerId, classSectionList[i].lecturer_id);
                Assert.AreEqual(classSectionDTOs[i].LecturerName, classSectionList[i].lecturer.full_name);
                Assert.AreEqual(classSectionDTOs[i].SubjectId, classSectionList[i].subject_id);
                Assert.AreEqual(classSectionDTOs[i].RoomId, classSectionList[i].room_id);
                Assert.AreEqual(classSectionDTOs[i].Subject, classSectionList[i].subject);
            }
        }

        [TestMethod()]
        public void Get_Data_In_Term_Return_Correct_Partial_View_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);

            // Act
            PartialViewResult result = controller.GetData(termId, "-1") as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("_Timetable", result.ViewName);
        }

        [TestMethod()]
        public void Get_Data_Partial_View_Should_Load_Class_In_Term_Not_Null_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);

            // Act
            PartialViewResult result = controller.GetData(termId, "-1") as PartialViewResult;
            object modelResult = result.Model;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(modelResult);
        }

        [TestMethod()]
        public void Get_Data_Partial_View_Should_Load_Class_Section_View_Models_In_Term_Equal_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);

            // Act
            PartialViewResult result = controller.GetData(termId, "-1") as PartialViewResult;
            TimetableViewModel modelResult = result.Model as TimetableViewModel;
            IEnumerable<ClassSectionDTO> classSectionDTOs = modelResult.ClassSectionDTOs;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(listClassSection.Count(), classSectionDTOs.Count());
        }

        [TestMethod()]
        public void Get_Data_Partial_View_Should_Load_Class_Section_View_Models_In_Term_Data_Correctly_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);

            // Act
            PartialViewResult result = controller.GetData(termId, "-1") as PartialViewResult;
            TimetableViewModel modelResult = result.Model as TimetableViewModel;
            List<ClassSectionDTO> classSectionDTOs = modelResult.ClassSectionDTOs.ToList();
            List<class_section> classSectionList = listClassSection.ToList();

            // Assert
            Assert.IsNotNull(result);
            for (int i = 0; i < classSectionList.Count(); i++)
            {
                Assert.AreEqual(classSectionDTOs[i].Id, classSectionList[i].id);
                Assert.AreEqual(classSectionDTOs[i].ClassSectionId, classSectionList[i].class_section_id);
                Assert.AreEqual(classSectionDTOs[i].Type, classSectionList[i].type);
                Assert.AreEqual(classSectionDTOs[i].Day2, classSectionList[i].day_2);
                Assert.AreEqual(classSectionDTOs[i].StartLesson2, classSectionList[i].start_lesson_2);
                Assert.AreEqual(classSectionDTOs[i].StudentRegisteredNumber, classSectionList[i].student_registered_number);
                Assert.AreEqual(classSectionDTOs[i].LastAssigned, classSectionList[i].lecturer1.full_name);
                Assert.AreEqual(classSectionDTOs[i].LecturerId, classSectionList[i].lecturer_id);
                Assert.AreEqual(classSectionDTOs[i].LecturerName, classSectionList[i].lecturer.full_name);
                Assert.AreEqual(classSectionDTOs[i].SubjectId, classSectionList[i].subject_id);
                Assert.AreEqual(classSectionDTOs[i].RoomId, classSectionList[i].room_id);
                Assert.AreEqual(classSectionDTOs[i].Subject, classSectionList[i].subject);
            }
        }

        [TestMethod()]
        public void Get_Data_Partial_View_Should_Load_Class_Section_View_Models_Days_Not_Null_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);

            // Act
            PartialViewResult result = controller.GetData(termId, "-1") as PartialViewResult;
            TimetableViewModel modelResult = result.Model as TimetableViewModel;
            List<int> timetableDays = modelResult.days;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(modelResult);
            Assert.IsNotNull(timetableDays);
        }

        [TestMethod()]
        public void Get_Data_Partial_View_Should_Load_Class_Section_View_Models_Days_Equal_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            List<int> daysList = new TimetableViewModel().days;

            // Act
            PartialViewResult result = controller.GetData(termId, "-1") as PartialViewResult;
            TimetableViewModel modelResult = result.Model as TimetableViewModel;
            List<int> timetableDays = modelResult.days;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(daysList.Count(), timetableDays.Count());
        }

        [TestMethod()]
        public void Get_Data_Partial_View_Should_Load_Class_Section_View_Models_Days_Correctly_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            List<int> daysList = new TimetableViewModel().days;

            // Act
            PartialViewResult result = controller.GetData(termId, "-1") as PartialViewResult;
            TimetableViewModel modelResult = result.Model as TimetableViewModel;
            List<int> timetableDays = modelResult.days;

            // Assert
            Assert.IsNotNull(result);
            for (int i = 0; i < daysList.Count(); i++)
            {
                Assert.AreEqual(timetableDays[i], daysList[i]);
            }
        }

        [TestMethod()]
        public void Get_Data_Partial_View_Should_Load_Class_Section_View_Models_Start_Lessons_Not_Null_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);

            // Act
            PartialViewResult result = controller.GetData(termId, "-1") as PartialViewResult;
            TimetableViewModel modelResult = result.Model as TimetableViewModel;
            List<int> timetableStartLessons = modelResult.startLessons;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(modelResult);
            Assert.IsNotNull(timetableStartLessons);
        }

        [TestMethod()]
        public void Get_Data_Partial_View_Should_Load_Class_Section_View_Models_Start_Lessons_Equal_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            List<int> startLessonsList = new TimetableViewModel().startLessons;

            // Act
            PartialViewResult result = controller.GetData(termId, "-1") as PartialViewResult;
            TimetableViewModel modelResult = result.Model as TimetableViewModel;
            List<int> timetableStartLessons = modelResult.startLessons;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(startLessonsList.Count(), timetableStartLessons.Count());
        }

        [TestMethod()]
        public void Get_Data_Partial_View_Should_Load_Class_Section_View_Models_Start_Lessons_Correctly_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            List<int> startLessonsList = new TimetableViewModel().startLessons;

            // Act
            PartialViewResult result = controller.GetData(termId, "-1") as PartialViewResult;
            TimetableViewModel modelResult = result.Model as TimetableViewModel;
            List<int> timetableStartLessons = modelResult.startLessons;

            // Assert
            Assert.IsNotNull(result);
            for (int i = 0; i < startLessonsList.Count(); i++)
            {
                Assert.AreEqual(timetableStartLessons[i], startLessonsList[i]);
            }
        }

        [TestMethod()]
        public void Import_View_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController();

            // Act
            ViewResult result = controller.Import() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void Import_View_Should_Load_Term_SelectList_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);

            // Act
            ViewResult result = controller.Import() as ViewResult;
            SelectList termList = new SelectList(listTerm);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(termList.Count(), ((IEnumerable<dynamic>)result.ViewBag.term).Count());
        }

        [TestMethod()]
        public void Import_View_Should_Load_Term_SelectList_Data_Correctly_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);

            // Act
            ViewResult result = controller.Import() as ViewResult;
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
        public void Import_View_Should_Load_Major_SelectList_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);

            // Act
            ViewResult result = controller.Import() as ViewResult;
            SelectList majorList = new SelectList(listMajor);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(majorList.Count(), ((IEnumerable<dynamic>)result.ViewBag.major).Count());
        }

        [TestMethod()]
        public void Import_View_Should_Load_Major_SelectList_Data_Correctly_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);

            // Act
            ViewResult result = controller.Import() as ViewResult;
            dynamic viewBagResult = result.ViewBag.major.Items;
            List<major> majorList = listMajor.ToList();

            // Assert
            Assert.IsNotNull(result);
            for (int i = 0; i < majorList.Count(); i++)
            {
                Assert.AreEqual(viewBagResult[i].id, majorList[i].id);
                Assert.AreEqual(viewBagResult[i].name, majorList[i].name);
                Assert.AreEqual(viewBagResult[i].abbreviation, majorList[i].abbreviation);
            }
        }

        [TestMethod()]
        public void Assign_View_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController();

            // Act
            ViewResult result = controller.Assign() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void Assign_View_Should_Load_Term_SelectList_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);

            // Act
            ViewResult result = controller.Assign() as ViewResult;
            SelectList termList = new SelectList(listTerm);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(termList.Count(), ((IEnumerable<dynamic>)result.ViewBag.term).Count());
        }

        [TestMethod()]
        public void Assign_View_Should_Load_Term_SelectList_Data_Correctly_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);

            // Act
            ViewResult result = controller.Assign() as ViewResult;
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
        public void Assign_View_Should_Load_Major_SelectList_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);

            // Act
            ViewResult result = controller.Assign() as ViewResult;
            SelectList majorList = new SelectList(listMajor);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(majorList.Count(), ((IEnumerable<dynamic>)result.ViewBag.major).Count());
        }

        [TestMethod()]
        public void Assign_View_Should_Load_Major_SelectList_Data_Correctly_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);

            // Act
            ViewResult result = controller.Assign() as ViewResult;
            dynamic viewBagResult = result.ViewBag.major.Items;
            List<major> majorList = listMajor.ToList();

            // Assert
            Assert.IsNotNull(result);
            for (int i = 0; i < majorList.Count(); i++)
            {
                Assert.AreEqual(viewBagResult[i].id, majorList[i].id);
                Assert.AreEqual(viewBagResult[i].name, majorList[i].name);
                Assert.AreEqual(viewBagResult[i].abbreviation, majorList[i].abbreviation);
            }
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Success_Not_Null_When_Lecturer_Id_String_Is_Empty_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            JsonResult actionResult = controller.CheckState(classSection.id, termId, string.Empty, false);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            Assert.IsNotNull(jsonCollection);
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Success_When_Lecturer_Id_String_Is_Empty_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            JsonResult actionResult = controller.CheckState(classSection.id, termId, string.Empty, false);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            Assert.IsTrue(jsonCollection.success);
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Success_Not_Null_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId2, false);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            Assert.IsNotNull(jsonCollection);
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Success_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId2, false);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            Assert.IsTrue(jsonCollection.success);
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Duplicate_Class_Error_Not_Null_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, false);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            Assert.IsNotNull(jsonCollection);
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Duplicate_Class_Error_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, false);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            Assert.IsFalse(jsonCollection.success);
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Duplicate_Class_Error_List_Not_Null_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, false);
            dynamic jsonCollection = actionResult.Data;
            dynamic classList = jsonCollection.classList;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            Assert.IsNotNull(classList);
            foreach (dynamic json in classList)
            {
                Assert.IsNotNull(json.classId);
                Assert.IsNotNull(json.subjectName);
                Assert.IsNotNull(json.classDay);
                Assert.IsNotNull(json.lessonTime);
                Assert.IsNotNull(json.majorName);
            }
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Duplicate_Class_Error_List_Not_False_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, false);
            dynamic jsonCollection = actionResult.Data;
            dynamic classList = jsonCollection.classList;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            foreach (dynamic json in classList)
            {
                Assert.IsNotNull(json.classId,
                    "JSON record does not contain \"classId\" required property.");
                Assert.IsNotNull(json.subjectName,
                    "JSON record does not contain \"subjectName\" required property.");
                Assert.IsNotNull(json.classDay,
                    "JSON record does not contain \"classDay\" required property.");
                Assert.IsNotNull(json.lessonTime,
                    "JSON record does not contain \"lessonTime\" required property.");
                Assert.IsNotNull(json.majorName,
                    "JSON record does not contain \"majorName\" required property.");
            }
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Duplicate_Class_Error_List_Should_Convert_To_IEnumerable_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, false);
            dynamic jsonCollection = actionResult.Data;
            dynamic classList = jsonCollection.classList;
            int count = 0;
            foreach (dynamic value in classList)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Duplicate_Class_Error_List_Index_At_0_Shoud_Not_Be_Null_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, false);
            dynamic jsonCollection = actionResult.Data;
            dynamic classList = jsonCollection.classList;

            // Assert
            Assert.IsNotNull(classList[0]);
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Duplicate_Class_Error_List_Should_Be_Indexable_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, false);
            dynamic jsonCollection = actionResult.Data;
            dynamic classList = jsonCollection.classList;

            // Assert
            for (int i = 0; i < classList.Count; i++)
            {

                dynamic json = classList[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.classId,
                    "JSON record does not contain \"classId\" required property.");
                Assert.IsNotNull(json.subjectName,
                    "JSON record does not contain \"subjectName\" required property.");
                Assert.IsNotNull(json.classDay,
                    "JSON record does not contain \"classDay\" required property.");
                Assert.IsNotNull(json.lessonTime,
                    "JSON record does not contain \"lessonTime\" required property.");
                Assert.IsNotNull(json.majorName,
                    "JSON record does not contain \"majorName\" required property.");
            }
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Maximum_Lesson_Error_Not_Null_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            term.max_lesson = 3;
            classSection.lecturer_id = null;
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, false);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            Assert.IsNotNull(jsonCollection);
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Maximum_Lesson_Error_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            term.max_lesson = 3;
            classSection.lecturer_id = null;
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, false);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            Assert.IsFalse(jsonCollection.success);
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Maximum_Lesson_Error_List_Not_Null_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            term.max_lesson = 3;
            classSection.lecturer_id = null;
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, false);
            dynamic jsonCollection = actionResult.Data;
            dynamic classList = jsonCollection.classList;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            Assert.IsNotNull(classList);
            foreach (dynamic json in classList)
            {
                Assert.IsNotNull(json.classId);
                Assert.IsNotNull(json.subjectName);
                Assert.IsNotNull(json.classDay);
                Assert.IsNotNull(json.lessonTime);
                Assert.IsNotNull(json.majorName);
            }
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Maximum_Lesson_Error_List_Not_False_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            term.max_lesson = 3;
            classSection.lecturer_id = null;
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, false);
            dynamic jsonCollection = actionResult.Data;
            dynamic classList = jsonCollection.classList;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            foreach (dynamic json in classList)
            {
                Assert.IsNotNull(json.classId,
                    "JSON record does not contain \"classId\" required property.");
                Assert.IsNotNull(json.subjectName,
                    "JSON record does not contain \"subjectName\" required property.");
                Assert.IsNotNull(json.classDay,
                    "JSON record does not contain \"classDay\" required property.");
                Assert.IsNotNull(json.lessonTime,
                    "JSON record does not contain \"lessonTime\" required property.");
                Assert.IsNotNull(json.majorName,
                    "JSON record does not contain \"majorName\" required property.");
            }
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Maximum_Lesson_Error_List_Should_Convert_To_IEnumerable_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            term.max_lesson = 3;
            classSection.lecturer_id = null;
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, false);
            dynamic jsonCollection = actionResult.Data;
            dynamic classList = jsonCollection.classList;
            int count = 0;
            foreach (dynamic value in classList)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Maximum_Lesson_Error_List_Index_At_0_Shoud_Not_Be_Null_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            term.max_lesson = 3;
            classSection.lecturer_id = null;
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, false);
            dynamic jsonCollection = actionResult.Data;
            dynamic classList = jsonCollection.classList;

            // Assert
            Assert.IsNotNull(classList[0]);
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Maximum_Lesson_Error_List_Should_Be_Indexable_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            term.max_lesson = 3;
            classSection.lecturer_id = null;
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, false);
            dynamic jsonCollection = actionResult.Data;
            dynamic classList = jsonCollection.classList;

            // Assert
            for (int i = 0; i < classList.Count; i++)
            {

                dynamic json = classList[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.classId,
                    "JSON record does not contain \"classId\" required property.");
                Assert.IsNotNull(json.subjectName,
                    "JSON record does not contain \"subjectName\" required property.");
                Assert.IsNotNull(json.classDay,
                    "JSON record does not contain \"classDay\" required property.");
                Assert.IsNotNull(json.lessonTime,
                    "JSON record does not contain \"lessonTime\" required property.");
                Assert.IsNotNull(json.majorName,
                    "JSON record does not contain \"majorName\" required property.");
            }
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Maximum_Class_Error_Not_Null_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            term.max_class = 1;
            classSection.lecturer_id = null;
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, false);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            Assert.IsNotNull(jsonCollection);
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Maximum_Class_Error_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            term.max_class = 1;
            classSection.lecturer_id = null;
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, false);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            Assert.IsFalse(jsonCollection.success);
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Maximum_Class_Error_List_Not_Null_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            term.max_class = 1;
            classSection.lecturer_id = null;
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, false);
            dynamic jsonCollection = actionResult.Data;
            dynamic classList = jsonCollection.classList;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            Assert.IsNotNull(classList);
            foreach (dynamic json in classList)
            {
                Assert.IsNotNull(json.classId);
                Assert.IsNotNull(json.subjectName);
                Assert.IsNotNull(json.classDay);
                Assert.IsNotNull(json.lessonTime);
                Assert.IsNotNull(json.majorName);
            }
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Maximum_Class_Error_List_Not_False_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            term.max_class = 1;
            classSection.lecturer_id = null;
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, false);
            dynamic jsonCollection = actionResult.Data;
            dynamic classList = jsonCollection.classList;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            foreach (dynamic json in classList)
            {
                Assert.IsNotNull(json.classId,
                    "JSON record does not contain \"classId\" required property.");
                Assert.IsNotNull(json.subjectName,
                    "JSON record does not contain \"subjectName\" required property.");
                Assert.IsNotNull(json.classDay,
                    "JSON record does not contain \"classDay\" required property.");
                Assert.IsNotNull(json.lessonTime,
                    "JSON record does not contain \"lessonTime\" required property.");
                Assert.IsNotNull(json.majorName,
                    "JSON record does not contain \"majorName\" required property.");
            }
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Maximum_Class_Error_List_Should_Convert_To_IEnumerable_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            term.max_class = 1;
            classSection.lecturer_id = null;
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, false);
            dynamic jsonCollection = actionResult.Data;
            dynamic classList = jsonCollection.classList;
            int count = 0;
            foreach (dynamic value in classList)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Maximum_Class_Error_List_Index_At_0_Shoud_Not_Be_Null_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            term.max_class = 1;
            classSection.lecturer_id = null;
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, false);
            dynamic jsonCollection = actionResult.Data;
            dynamic classList = jsonCollection.classList;

            // Assert
            Assert.IsNotNull(classList[0]);
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Maximum_Class_Error_List_Should_Be_Indexable_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            term.max_class = 1;
            classSection.lecturer_id = null;
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, false);
            dynamic jsonCollection = actionResult.Data;
            dynamic classList = jsonCollection.classList;

            // Assert
            for (int i = 0; i < classList.Count; i++)
            {

                dynamic json = classList[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.classId,
                    "JSON record does not contain \"classId\" required property.");
                Assert.IsNotNull(json.subjectName,
                    "JSON record does not contain \"subjectName\" required property.");
                Assert.IsNotNull(json.classDay,
                    "JSON record does not contain \"classDay\" required property.");
                Assert.IsNotNull(json.lessonTime,
                    "JSON record does not contain \"lessonTime\" required property.");
                Assert.IsNotNull(json.majorName,
                    "JSON record does not contain \"majorName\" required property.");
            }
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Different_Campus_Error_Not_Null_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            classSection.room_id = "CS4.F.04.01";
            classSection.lecturer_id = null;
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, true);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            Assert.IsNotNull(jsonCollection);
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Different_Campus_Error_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            classSection.room_id = "CS4.F.04.01";
            classSection.lecturer_id = null;
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, true);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            Assert.IsFalse(jsonCollection.success);
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Different_Campus_Error_List_Not_Null_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            classSection.room_id = "CS4.F.04.01";
            classSection.lecturer_id = null;
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, true);
            dynamic jsonCollection = actionResult.Data;
            dynamic classList = jsonCollection.classList;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            Assert.IsNotNull(classList);
            foreach (dynamic json in classList)
            {
                Assert.IsNotNull(json.classId);
                Assert.IsNotNull(json.subjectName);
                Assert.IsNotNull(json.classDay);
                Assert.IsNotNull(json.lessonTime);
                Assert.IsNotNull(json.roomId);
                Assert.IsNotNull(json.majorName);
            }
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Different_Campus_Error_List_Not_False_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            classSection.room_id = "CS4.F.04.01";
            classSection.lecturer_id = null;
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, true);
            dynamic jsonCollection = actionResult.Data;
            dynamic classList = jsonCollection.classList;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            foreach (dynamic json in classList)
            {
                Assert.IsNotNull(json.classId,
                    "JSON record does not contain \"classId\" required property.");
                Assert.IsNotNull(json.subjectName,
                    "JSON record does not contain \"subjectName\" required property.");
                Assert.IsNotNull(json.classDay,
                    "JSON record does not contain \"classDay\" required property.");
                Assert.IsNotNull(json.lessonTime,
                    "JSON record does not contain \"lessonTime\" required property.");
                Assert.IsNotNull(json.roomId,
                    "JSON record does not contain \"roomId\" required property.");
                Assert.IsNotNull(json.majorName,
                    "JSON record does not contain \"majorName\" required property.");
            }
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Different_Campus_Error_List_Should_Convert_To_IEnumerable_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            classSection.room_id = "CS4.F.04.01";
            classSection.lecturer_id = null;
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, true);
            dynamic jsonCollection = actionResult.Data;
            dynamic classList = jsonCollection.classList;
            int count = 0;
            foreach (dynamic value in classList)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Different_Campus_Error_List_Index_At_0_Shoud_Not_Be_Null_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            classSection.room_id = "CS4.F.04.01";
            classSection.lecturer_id = null;
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, true);
            dynamic jsonCollection = actionResult.Data;
            dynamic classList = jsonCollection.classList;

            // Assert
            Assert.IsNotNull(classList[0]);
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Different_Campus_Error_List_Should_Be_Indexable_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            classSection.room_id = "CS4.F.04.01";
            classSection.lecturer_id = null;
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, true);
            dynamic jsonCollection = actionResult.Data;
            dynamic classList = jsonCollection.classList;

            // Assert
            for (int i = 0; i < classList.Count; i++)
            {

                dynamic json = classList[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.classId,
                    "JSON record does not contain \"classId\" required property.");
                Assert.IsNotNull(json.subjectName,
                    "JSON record does not contain \"subjectName\" required property.");
                Assert.IsNotNull(json.classDay,
                    "JSON record does not contain \"classDay\" required property.");
                Assert.IsNotNull(json.lessonTime,
                    "JSON record does not contain \"lessonTime\" required property.");
                Assert.IsNotNull(json.roomId,
                    "JSON record does not contain \"roomId\" required property.");
                Assert.IsNotNull(json.majorName,
                    "JSON record does not contain \"majorName\" required property.");
            }
        }

        [TestMethod()]
        public void Get_Check_State_Json_Data_Different_Campus_Should_Return_Success_If_Warning_If_False_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            term term = listTerm.First();
            class_section classSection = listClassSection.First();
            mockSetTerm.Setup(m => m.Find(It.IsAny<int>())).Returns(term);
            mockSetClassSection.Setup(m => m.Find(It.IsAny<int>())).Returns(classSection);

            // Act
            classSection.room_id = "CS4.F.04.01";
            classSection.lecturer_id = null;
            JsonResult actionResult = controller.CheckState(classSection.id, termId, userId1, false);
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            Assert.IsTrue(jsonCollection.success);
        }

        [TestMethod()]
        public void Delete_Class_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);

            // Act
            JsonResult result = controller.Delete(listClassSection.First().id) as JsonResult;
            dynamic jsonCollection = result.Data;

            // Assert
            Assert.IsNotNull(jsonCollection);
            mockSetClassSection.Verify(r => r.Remove(It.IsAny<class_section>()));
            mockContext.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod()]
        public void Delete_All_Class_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);

            // Act
            JsonResult result = controller.DeleteAll(termId, majorId) as JsonResult;
            dynamic jsonCollection = result.Data;

            // Assert
            Assert.IsNotNull(jsonCollection);
            mockSetClassSection.Verify(r => r.RemoveRange(It.IsAny<IEnumerable<class_section>>()));
            mockContext.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod()]
        public void Validate_Columns_Should_Return_Null_If_Valid_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            DataTable table = new DataTable();

            // Act
            table.Columns.AddRange(new DataColumn[30]
            {
                new DataColumn("MaGocLHP"), new DataColumn("Mã MH"), new DataColumn("Mã LHP"), new DataColumn("Tên HP"),
                new DataColumn("Số TC"), new DataColumn("Loại HP"), new DataColumn("Mã Lớp"), new DataColumn("TSMH"),
                new DataColumn("Số Tiết Đã xếp"), new DataColumn("PH"), new DataColumn("Thứ"), new DataColumn("Tiết BĐ"),
                new DataColumn("Số Tiết"), new DataColumn("Tiết Học"), new DataColumn("Phòng"),
                new DataColumn("Mã CBGD"), new DataColumn("Tên CBGD"), new DataColumn("PH_X"),
                new DataColumn("Sức Chứa"), new DataColumn("SiSoTKB"), new DataColumn("Trống"),
                new DataColumn("Tình Trạng LHP"), new DataColumn("TuanHoc2"), new DataColumn("ThuS"),
                new DataColumn("TietS"), new DataColumn("Số SVĐK"), new DataColumn("Tuần BD"), new DataColumn("Tuần KT"),
                new DataColumn("Ghi Chú 1"), new DataColumn("Ghi Chú 2")
            });

            string result = controller.ValidateColumns(table);

            // Assert
            Assert.AreEqual(30, table.Columns.Count);
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void Validate_Columns_Should_Return_Not_Null_If_Missing_Columns_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            DataTable table = new DataTable();

            // Act
            table.Columns.AddRange(new DataColumn[29]
            {
                new DataColumn("MaGocLHP"), new DataColumn("Mã MH"), new DataColumn("Mã LHP"), new DataColumn("Tên HP"),
                new DataColumn("Số TC"), new DataColumn("Loại HP"), new DataColumn("Mã Lớp"), new DataColumn("TSMH"),
                new DataColumn("Số Tiết Đã xếp"), new DataColumn("PH"), new DataColumn("Thứ"), new DataColumn("Tiết BĐ"),
                new DataColumn("Số Tiết"), new DataColumn("Tiết Học"), new DataColumn("Phòng"),
                new DataColumn("Mã CBGD"), new DataColumn("Tên CBGD"), new DataColumn("PH_X"),
                new DataColumn("Sức Chứa"), new DataColumn("SiSoTKB"), new DataColumn("Trống"),
                new DataColumn("Tình Trạng LHP"), new DataColumn("TuanHoc2"), new DataColumn("ThuS"),
                new DataColumn("TietS"), new DataColumn("Số SVĐK"), new DataColumn("Tuần BD"), new DataColumn("Tuần KT"),
                new DataColumn("Ghi Chú 1")
            });

            string result = controller.ValidateColumns(table);

            // Assert
            Assert.AreEqual(29, table.Columns.Count);
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void Validate_Columns_Should_Return_Missing_Column_Name_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            DataTable table = new DataTable();
            string missingColumn = "Ghi chú 2";

            // Act
            table.Columns.AddRange(new DataColumn[29]
            {
                new DataColumn("MaGocLHP"), new DataColumn("Mã MH"), new DataColumn("Mã LHP"), new DataColumn("Tên HP"),
                new DataColumn("Số TC"), new DataColumn("Loại HP"), new DataColumn("Mã Lớp"), new DataColumn("TSMH"),
                new DataColumn("Số Tiết Đã xếp"), new DataColumn("PH"), new DataColumn("Thứ"), new DataColumn("Tiết BĐ"),
                new DataColumn("Số Tiết"), new DataColumn("Tiết Học"), new DataColumn("Phòng"),
                new DataColumn("Mã CBGD"), new DataColumn("Tên CBGD"), new DataColumn("PH_X"),
                new DataColumn("Sức Chứa"), new DataColumn("SiSoTKB"), new DataColumn("Trống"),
                new DataColumn("Tình Trạng LHP"), new DataColumn("TuanHoc2"), new DataColumn("ThuS"),
                new DataColumn("TietS"), new DataColumn("Số SVĐK"), new DataColumn("Tuần BD"), new DataColumn("Tuần KT"),
                new DataColumn("Ghi Chú 1")
            });

            string result = controller.ValidateColumns(table);

            // Assert
            Assert.AreEqual(29, table.Columns.Count);
            Assert.AreEqual(result, missingColumn);
        }

        [TestMethod()]
        public void Validate_Columns_Should_Return_First_Missing_Column_Name_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            DataTable table = new DataTable();
            string missingColumn = "Ghi Chú 1";

            // Act
            table.Columns.AddRange(new DataColumn[28]
            {
                new DataColumn("MaGocLHP"), new DataColumn("Mã MH"), new DataColumn("Mã LHP"), new DataColumn("Tên HP"),
                new DataColumn("Số TC"), new DataColumn("Loại HP"), new DataColumn("Mã Lớp"), new DataColumn("TSMH"),
                new DataColumn("Số Tiết Đã xếp"), new DataColumn("PH"), new DataColumn("Thứ"), new DataColumn("Tiết BĐ"),
                new DataColumn("Số Tiết"), new DataColumn("Tiết Học"), new DataColumn("Phòng"),
                new DataColumn("Mã CBGD"), new DataColumn("Tên CBGD"), new DataColumn("PH_X"),
                new DataColumn("Sức Chứa"), new DataColumn("SiSoTKB"), new DataColumn("Trống"),
                new DataColumn("Tình Trạng LHP"), new DataColumn("TuanHoc2"), new DataColumn("ThuS"),
                new DataColumn("TietS"), new DataColumn("Số SVĐK"), new DataColumn("Tuần BD"), new DataColumn("Tuần KT")
            });

            string result = controller.ValidateColumns(table);

            // Assert
            Assert.AreEqual(28, table.Columns.Count);
            Assert.AreEqual(result, missingColumn);
        }

        [TestMethod()]
        public void Validate_Not_Null_Should_Return_Null_If_Valid_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);

            // Act
            string[] validRows = { "Hả hẻ", "Haha", "Hố hố" };
            string result = controller.ValidateNotNull(validRows);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void Validate_Not_Null_Should_Return_Null_If_Array_Is_Null_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);

            // Act
            string[] validRows = { };
            string result = controller.ValidateNotNull(validRows);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod()]
        public void Validate_Not_Null_Should_Return_Not_Null_If_Not_Valid_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);

            // Act
            string[] validRows = { "Hả hẻ", "Haha", string.Empty };
            string result = controller.ValidateNotNull(validRows);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void Validate_Not_Null_Should_Return_Empty_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);

            // Act
            string[] validRows = { "Hả hẻ", "Haha", string.Empty };
            string result = controller.ValidateNotNull(validRows);

            // Assert
            Assert.AreEqual(result, string.Empty);
        }

        [TestMethod()]
        public void Return_Original_String_When_Not_Empty_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            string test = "Nguyễn Văn A";

            // Act
            string result = controller.ToNullableString(test);

            // Assert
            Assert.AreEqual(test, result);
        }

        [TestMethod()]
        public void Set_Null_String_On_Empty_String_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            string test = "";

            // Act
            string result = controller.ToNullableString(test);

            // Assert
            Assert.AreEqual(null, result);
        }

        [TestMethod()]
        public void Set_Null_String_On_Empty_String_Type_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            string test = string.Empty;

            // Act
            string result = controller.ToNullableString(test);

            // Assert
            Assert.AreEqual(null, result);
        }

        [TestMethod()]
        public void String_Should_Convert_To_Null_If_Have_Space_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            string test = "      ";

            // Act
            string result = controller.ToNullableString(test);

            // Assert
            Assert.AreEqual(null, result);
        }

        [TestMethod()]
        public void Return_Int_When_Not_Empty_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            string stringTest = "123";
            int intTest = int.Parse(stringTest);

            // Act
            int? result = controller.ToNullableInt(stringTest);

            // Assert
            Assert.AreEqual(intTest, result);
        }

        [TestMethod()]
        public void Set_Null_On_Empty_Int_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            string test = "";

            // Act
            int? result = controller.ToNullableInt(test);

            // Assert
            Assert.AreEqual(null, result);
        }

        [TestMethod()]
        public void Set_Null_Int_On_Empty_String_Type_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            string test = string.Empty;

            // Act
            int? result = controller.ToNullableInt(test);

            // Assert
            Assert.AreEqual(null, result);
        }

        [TestMethod()]
        public void String_Should_Convert_To_Null_Int_If_Have_Space_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            string test = "      ";

            // Act
            int? result = controller.ToNullableInt(test);

            // Assert
            Assert.AreEqual(null, result);
        }

        [TestMethod()]
        public void String_Should_Convert_To_Int_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController(unitOfWork);
            string stringTest = "123";
            int intTest = int.Parse(stringTest);

            // Act
            int result = controller.ToInt(stringTest);

            // Assert
            Assert.AreEqual(intTest, result);
        }
    }
}