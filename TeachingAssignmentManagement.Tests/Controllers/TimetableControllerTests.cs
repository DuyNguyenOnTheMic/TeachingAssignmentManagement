using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        private IQueryable<class_section> listClassSection;
        private IQueryable<term> listTerm;
        private Mock<DbSet<term>> mockSetTerm;
        private Mock<CP25Team03Entities> mockContext;
        private UnitOfWork unitOfWork;

        [TestInitialize()]
        public void Initialize()
        {
            listClassSection = new List<class_section>
            {
                new class_section { id = 1, original_id = "221_71ITBS10103_01", class_section_id = "221_71ITBS10103_01" },
            }.AsQueryable();
            listTerm = new List<term> {
                new term() { id = 123, start_year = 2022, end_year = 2023, start_week = 1, start_date = DateTime.Now, max_lesson = 6, max_class = 6 },
                new term() { id = 124, start_year = 2023, end_year = 2024, start_week = 1, start_date = DateTime.Now, max_lesson = 6, max_class = 6 }
            }.AsQueryable();
            mockSetTerm = new Mock<DbSet<term>>();
            mockContext = new Mock<CP25Team03Entities>();
            unitOfWork = new UnitOfWork(mockContext.Object);
            mockSetTerm.As<IQueryable<term>>().Setup(m => m.Provider).Returns(listTerm.Provider);
            mockSetTerm.As<IQueryable<term>>().Setup(m => m.Expression).Returns(listTerm.Expression);
            mockSetTerm.As<IQueryable<term>>().Setup(m => m.ElementType).Returns(listTerm.ElementType);
            mockSetTerm.As<IQueryable<term>>().Setup(m => m.GetEnumerator()).Returns(listTerm.GetEnumerator());
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
    }
}