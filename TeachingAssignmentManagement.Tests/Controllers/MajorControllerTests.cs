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
        private IQueryable<major> listMajor;
        private Mock<DbSet<major>> mockSet;
        private Mock<CP25Team03Entities> mockContext;
        private UnitOfWork unitOfWork;

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
            mockSet.As<IQueryable<major>>().Setup(m => m.Provider).Returns(listMajor.Provider);
            mockSet.As<IQueryable<major>>().Setup(m => m.Expression).Returns(listMajor.Expression);
            mockSet.As<IQueryable<major>>().Setup(m => m.ElementType).Returns(listMajor.ElementType);
            mockSet.As<IQueryable<major>>().Setup(m => m.GetEnumerator()).Returns(listMajor.GetEnumerator());
            mockContext.Setup(c => c.majors).Returns(() => mockSet.Object);
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
        public void Get_Major_Json_Data_Not_Null_Test()
        {
            // Arrange
            var controller = new MajorController(unitOfWork);

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
        public void Get_Major_Json_Data_Correctly_Test()
        {
            // Arrange.
            var controller = new MajorController(unitOfWork);

            // Act.
            var actionResult = controller.GetData();

            // Assert.
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            dynamic jsonCollection = actionResult.Data;
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.id,
                    "JSON record does not contain \"id\" required property.");
                Assert.IsNotNull(json.name,
                    "JSON record does not contain \"name\" required property.");
            }
        }

        [TestMethod()]
        public void Major_Json_Data_Should_Convert_To_IEnumerable_Test()
        {
            // Arrange
            var controller = new MajorController(unitOfWork);

            // Act
            var actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;
            int count = 0;
            foreach (var value in jsonCollection)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod]
        public void Major_Json_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Arrange
            var controller = new MajorController(unitOfWork);

            // Act
            var actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;

            // Assert                
            Assert.IsNotNull(jsonCollection[0]);
        }

        [TestMethod]
        public void Major_JSon_Data_Should_Be_Indexable_Test()
        {
            // Arrange
            var controller = new MajorController(unitOfWork);

            // Act
            var actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;

            // Assert
            for (var i = 0; i < jsonCollection.Count; i++)
            {

                var json = jsonCollection[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.id,
                   "JSON record does not contain \"id\" required property.");
                Assert.IsNotNull(json.name,
                    "JSON record does not contain \"name\" required property.");
            }
        }

        [TestMethod()]
        public void Get_Major_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Arrange
            var controller = new MajorController(unitOfWork);

            // Act
            var actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(jsonCollection);
            Assert.AreEqual(listMajor.Count(), jsonCollection.Count);
        }

        [TestMethod()]
        public void Create_View_Test()
        {
            // Arrange
            var controller = new MajorController();

            // Act
            ViewResult result = controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void Create_Major_Test()
        {
            // Arrange
            var major = new major() { id = "122", name = "hehe" };
            var controller = new MajorController(unitOfWork);

            // Act
            controller.Create(major);

            // Assert
            mockSet.Verify(r => r.Add(major), Times.Once);
            mockContext.Verify(r => r.SaveChanges(), Times.Once);
        }

        #region RepositoryTests

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
        #endregion
    }
}