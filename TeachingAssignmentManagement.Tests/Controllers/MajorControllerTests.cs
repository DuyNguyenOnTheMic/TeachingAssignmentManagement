using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Controllers.Tests
{
    [TestClass()]
    public class MajorControllerTests
    {
        private Mock<DbSet<major>> mockSet;
        private Mock<CP25Team03Entities> mockContext;
        private UnitOfWork unitOfWork;
        private MajorController controller;
        private IQueryable<major> listMajor;

        [TestInitialize]
        public void Initialize()
        {
            listMajor = new List<major> {
                new major() { id = "1", name = "Kỹ Thuật Phần Mềm" },
                new major() { id = "2", name = "Công Nghệ Thông Tin" }
            }.AsQueryable();
            mockSet = new Mock<DbSet<major>>();
            mockContext = new Mock<CP25Team03Entities>();
            unitOfWork = new UnitOfWork(mockContext.Object);
            controller = new MajorController(unitOfWork);
            mockContext.Setup(c => c.majors).Returns(() => mockSet.Object);
            mockSet.As<IQueryable<major>>().Setup(m => m.Provider).Returns(listMajor.Provider);
            mockSet.As<IQueryable<major>>().Setup(m => m.Expression).Returns(listMajor.Expression);
            mockSet.As<IQueryable<major>>().Setup(m => m.ElementType).Returns(listMajor.ElementType);
            mockSet.As<IQueryable<major>>().Setup(m => m.GetEnumerator()).Returns(listMajor.GetEnumerator());
        }

        [TestMethod()]
        public void Index_Test()
        {
            // Arrange
            var controller = new MajorController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void Get_Major_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Act
            var major = unitOfWork.MajorRepository.GetMajors().Cast<object>().ToList();

            // Assert
            Assert.IsNotNull(major);
            Assert.AreEqual(listMajor.Count(), major.Count);
        }

        [TestMethod()]
        public void Get_Major_Json_Data_Not_Null_Test()
        {
            // Act
            var actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.id);
                Assert.IsNotNull(json.name);
            }
        }

        [TestMethod()]
        public void Insert_Major_Controller_Test()
        {
            // Arrange
            var major = new major() { id = "122", name = "hehe" };

            // Act
            controller.Create(major);

            // Assert
            mockSet.Verify(r => r.Add(major), Times.Once);
            mockContext.Verify(r => r.SaveChanges(), Times.Once);
        }


        [TestMethod()]
        public void Insert_Major_Repository_Test()
        {
            // Arrange
            var major = new major() { id = "122", name = "hehe" };

            // Act
            unitOfWork.MajorRepository.InsertMajor(major);
            unitOfWork.Save();

            // Assert
            mockSet.Verify(r => r.Add(major), Times.Once);
            mockContext.Verify(r => r.SaveChanges(), Times.Once);
        }
    }
}