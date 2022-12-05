using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
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
        private TransactionScope scope;

        [TestInitialize()]
        public void Initialize()
        {
            listMajor = new List<major> {
                new major() { id = "1", name = "Kỹ Thuật Phần Mềm" },
                new major() { id = "2", name = "Công Nghệ Thông Tin" }
            }.AsQueryable();
            mockSet = new Mock<DbSet<major>>();
            mockContext = new Mock<CP25Team03Entities>();
            unitOfWork = new UnitOfWork(mockContext.Object);
            scope = new TransactionScope();
            mockSet.As<IQueryable<major>>().Setup(m => m.Provider).Returns(listMajor.Provider);
            mockSet.As<IQueryable<major>>().Setup(m => m.Expression).Returns(listMajor.Expression);
            mockSet.As<IQueryable<major>>().Setup(m => m.ElementType).Returns(listMajor.ElementType);
            mockSet.As<IQueryable<major>>().Setup(m => m.GetEnumerator()).Returns(listMajor.GetEnumerator());
            mockContext.Setup(c => c.majors).Returns(() => mockSet.Object);
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            unitOfWork.Dispose();
            scope.Dispose();
        }

        [TestMethod()]
        public void Index_Test()
        {
            // Arrange
            MajorController controller = new MajorController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void Get_Major_Json_Data_Not_Null_Test()
        {
            // Arrange
            MajorController controller = new MajorController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetData();
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
        public void Get_Major_Json_Data_Is_Correct_Test()
        {
            // Arrange
            MajorController controller = new MajorController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;
            List<major> majorList = listMajor.ToList();

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            for (int i = 0; i < listMajor.Count(); i++)
            {
                Assert.AreEqual(jsonCollection[i].id, majorList[i].id);
                Assert.AreEqual(jsonCollection[i].name, majorList[i].name);
            }
        }

        [TestMethod()]
        public void Get_Major_Json_Data_Not_False_Test()
        {
            // Arrange.
            MajorController controller = new MajorController(unitOfWork);

            // Act.
            JsonResult actionResult = controller.GetData();

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
            MajorController controller = new MajorController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;
            int count = 0;
            foreach (dynamic value in jsonCollection)
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
            MajorController controller = new MajorController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;

            // Assert                
            Assert.IsNotNull(jsonCollection[0]);
        }

        [TestMethod]
        public void Major_JSon_Data_Should_Be_Indexable_Test()
        {
            // Arrange
            MajorController controller = new MajorController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;

            // Assert
            for (int i = 0; i < jsonCollection.Count; i++)
            {

                dynamic json = jsonCollection[i];

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
            MajorController controller = new MajorController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(jsonCollection);
            Assert.AreEqual(listMajor.Count(), jsonCollection.Count);
        }

        [TestMethod()]
        public void Create_View_Test()
        {
            // Arrange
            MajorController controller = new MajorController();

            // Act
            ViewResult result = controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void Create_Major_Test()
        {
            // Arrange
            MajorController controller = new MajorController(unitOfWork);
            major major = new major() { id = "122", name = "hehe" };

            // Act
            controller.Create(major);

            // Assert
            mockSet.Verify(r => r.Add(major), Times.Once);
            mockContext.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void Create_Shoud_Be_Failed_When_Major_Id_Is_Null_Test()
        {
            // Arrange
            MajorController controller = new MajorController();
            major major = new major() { id = null, name = "Test" };

            // Act
            JsonResult result = controller.Create(major) as JsonResult;
            dynamic jsonCollection = result.Data;

            // Assert
            Assert.AreEqual(true, jsonCollection.error);
        }

        [TestMethod]
        public void Create_Shoud_Be_Failed_When_Major_Name_Is_Null_Test()
        {
            // Arrange
            MajorController controller = new MajorController();
            major major = new major() { id = "blabla", name = null };

            // Act
            JsonResult result = controller.Create(major) as JsonResult;
            dynamic jsonCollection = result.Data;

            // Assert
            Assert.AreEqual(true, jsonCollection.error);
        }

        [TestMethod]
        public void Create_Shoud_Be_Failed_When_Major_Id_Over_50_Characters_Test()
        {
            // Arrange
            MajorController controller = new MajorController();
            major major = new major() { id = "usposueremisedaccumsanliguladiamatdsdasdsaddasasadd", name = "Test" };

            // Act
            JsonResult result = controller.Create(major) as JsonResult;
            dynamic jsonCollection = result.Data;

            // Assert
            Assert.AreEqual(51, major.id.Length);
            Assert.AreEqual(true, jsonCollection.error);
        }

        [TestMethod]
        public void Create_Shoud_Be_Failed_When_Major_Name_Over_255_Characters_Test()
        {
            // Arrange
            MajorController controller = new MajorController();
            major major = new major() { id = "test", name = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Mauris efficitur, massa non aliquet accumsan, ligula risus posuere mi, sed accumsan ligula diam at ante. Duis fermentum blandit ante, viverra convallis magna varius et. Sed congue ut elit vitae ufsaa" };

            // Act
            JsonResult result = controller.Create(major) as JsonResult;
            dynamic jsonCollection = result.Data;

            // Assert
            Assert.AreEqual(256, major.name.Length);
            Assert.AreEqual(true, jsonCollection.error);
        }

        [TestMethod]
        public void Create_Should_Be_Failed_When_Major_Exists_Test()
        {
            // Arrange
            MajorController controller = new MajorController();
            major major = new major() { id = "test", name = "Hệ thống thông tin" };

            // Act
            JsonResult result;
            dynamic jsonCollection;
            using (scope)
            {
                controller.Create(major);
                result = controller.Create(major) as JsonResult;
                jsonCollection = result.Data;
            }

            // Assert
            Assert.AreEqual(true, jsonCollection.error);
        }

        [TestMethod()]
        public void Edit_Major_View_Test()
        {
            // Arrange
            MajorController controller = new MajorController(unitOfWork);
            major major = new major() { id = "test", name = "Hệ thống thông tin" };

            // Act
            ViewResult result;
            using (scope)
            {
                controller.Create(major);
                result = controller.Edit(major.id) as ViewResult;
            }

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void Edit_Major_Data_Should_Load_Correctly_Test()
        {
            // Arrange
            MajorController controller = new MajorController();
            major major = new major() { id = "test", name = "Hệ thống thông tin" };

            // Act
            ViewResult result;
            using (scope)
            {
                controller.Create(major);
                result = controller.Edit(major.id) as ViewResult;
            }
            major model = result.Model as major;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.AreEqual(model.id, major.id);
            Assert.AreEqual(model.name, major.name);
        }

        [TestMethod]
        public void Edit_Major_Test()
        {
            // Arrange
            MajorController controller = new MajorController(unitOfWork);
            major major = new major() { id = "test", name = "Hệ thống thông tin" };

            // Act
            JsonResult result = controller.Edit(major) as JsonResult;
            dynamic jsonCollection = result.Data;

            // Assert
            Assert.AreEqual(true, jsonCollection.success);
            mockContext.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void Delete_Major_Test()
        {
            // Arrange
            MajorController controller = new MajorController(unitOfWork);

            // Act
            JsonResult result = controller.Delete(listMajor.First().id) as JsonResult;
            dynamic jsonCollection = result.Data;

            // Assert
            Assert.IsNotNull(jsonCollection);
            mockSet.Verify(r => r.Remove(It.IsAny<major>()));
            mockContext.Verify(r => r.SaveChanges(), Times.Once);
        }

        #region RepositoryTests

        [TestMethod()]
        public void Insert_Major_Repository_Test()
        {
            // Arrange
            major major = new major() { id = "122", name = "hehe" };

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