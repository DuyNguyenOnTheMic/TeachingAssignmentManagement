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
    public class AcademicDegreeControllerTests
    {
        private IQueryable<academic_degree> listAcademicDegree;
        private Mock<DbSet<academic_degree>> mockSet;
        private Mock<CP25Team03Entities> mockContext;
        private UnitOfWork unitOfWork;
        private TransactionScope scope;

        [TestInitialize()]
        public void Initialize()
        {
            listAcademicDegree = new List<academic_degree> {
                new academic_degree() { id = "1", name = "CN", level = 5 },
                new academic_degree() { id = "2", name = "THS", level = 4 }
            }.AsQueryable();
            mockSet = new Mock<DbSet<academic_degree>>();
            mockContext = new Mock<CP25Team03Entities>();
            unitOfWork = new UnitOfWork(mockContext.Object);
            scope = new TransactionScope();
            mockSet.As<IQueryable<academic_degree>>().Setup(m => m.Provider).Returns(listAcademicDegree.Provider);
            mockSet.As<IQueryable<academic_degree>>().Setup(m => m.Expression).Returns(listAcademicDegree.Expression);
            mockSet.As<IQueryable<academic_degree>>().Setup(m => m.ElementType).Returns(listAcademicDegree.ElementType);
            mockSet.As<IQueryable<academic_degree>>().Setup(m => m.GetEnumerator()).Returns(listAcademicDegree.GetEnumerator());
            mockContext.Setup(c => c.academic_degree).Returns(() => mockSet.Object);
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
            AcademicDegreeController controller = new AcademicDegreeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void Get_Academic_Degree_Json_Data_Not_Null_Test()
        {
            // Arrange
            AcademicDegreeController controller = new AcademicDegreeController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.id);
                Assert.IsNotNull(json.name);
                Assert.IsNotNull(json.level);
            }
        }

        [TestMethod()]
        public void Get_Academic_Degree_Json_Data_Is_Correct_Test()
        {
            // Arrange
            AcademicDegreeController controller = new AcademicDegreeController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;
            List<academic_degree> academicDegreeList = listAcademicDegree.ToList();

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            for (int i = 0; i < listAcademicDegree.Count(); i++)
            {
                Assert.AreEqual(jsonCollection[i].id, academicDegreeList[i].id);
                Assert.AreEqual(jsonCollection[i].name, academicDegreeList[i].name);
                Assert.AreEqual(jsonCollection[i].level, academicDegreeList[i].level);
            }
        }

        [TestMethod()]
        public void Get_Academic_Degree_Json_Data_Not_False_Test()
        {
            // Arrange
            AcademicDegreeController controller = new AcademicDegreeController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetData();

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            dynamic jsonCollection = actionResult.Data;
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.id,
                    "JSON record does not contain \"id\" required property.");
                Assert.IsNotNull(json.name,
                    "JSON record does not contain \"name\" required property.");
                Assert.IsNotNull(json.level,
                    "JSON record does not contain \"level\" required property.");
            }
        }

        [TestMethod()]
        public void Academic_Degree_Json_Data_Should_Convert_To_IEnumerable_Test()
        {
            // Arrange
            AcademicDegreeController controller = new AcademicDegreeController(unitOfWork);

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

        [TestMethod()]
        public void Academic_Degree_Json_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Arrange
            AcademicDegreeController controller = new AcademicDegreeController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;

            // Assert                
            Assert.IsNotNull(jsonCollection[0]);
        }

        [TestMethod()]
        public void Academic_Degree_JSon_Data_Should_Be_Indexable_Test()
        {
            // Arrange
            AcademicDegreeController controller = new AcademicDegreeController(unitOfWork);

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
                Assert.IsNotNull(json.level,
                    "JSON record does not contain \"level\" required property.");
            }
        }

        [TestMethod()]
        public void Get_Academic_Degree_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Arrange
            AcademicDegreeController controller = new AcademicDegreeController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(jsonCollection);
            Assert.AreEqual(listAcademicDegree.Count(), jsonCollection.Count);
        }

        [TestMethod()]
        public void Create_View_Test()
        {
            // Arrange
            AcademicDegreeController controller = new AcademicDegreeController();

            // Act
            ViewResult result = controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void Create_Academic_Degree_Test()
        {
            // Arrange
            AcademicDegreeController controller = new AcademicDegreeController(unitOfWork);
            academic_degree academicDegree = new academic_degree() { id = "3", name = "hehe", level = 3 };

            // Act
            controller.Create(academicDegree);

            // Assert
            mockSet.Verify(r => r.Add(academicDegree), Times.Once);
            mockContext.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod()]
        public void Create_Shoud_Be_Failed_When_Academic_Degree_Id_Is_Null_Test()
        {
            // Arrange
            AcademicDegreeController controller = new AcademicDegreeController();
            academic_degree academicDegree = new academic_degree() { id = null, name = "hehe", level = 3 };

            // Act
            JsonResult result;
            dynamic jsonCollection;
            using (scope)
            {
                controller.Create(academicDegree);
                result = controller.Create(academicDegree) as JsonResult;
                jsonCollection = result.Data;
            }

            // Assert
            Assert.AreEqual(true, jsonCollection.error);
        }

        [TestMethod()]
        public void Create_Shoud_Be_Failed_When_Academic_Degree_Name_Over_100_Characters_Test()
        {
            // Arrange
            AcademicDegreeController controller = new AcademicDegreeController();
            academic_degree academicDegree = new academic_degree() { id = "1", name = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras consectetur sed tellus at tincidundsdss", level = 3 };

            // Act
            JsonResult result;
            dynamic jsonCollection;
            using (scope)
            {
                controller.Create(academicDegree);
                result = controller.Create(academicDegree) as JsonResult;
                jsonCollection = result.Data;
            }

            // Assert
            Assert.AreEqual(101, academicDegree.name.Length);
            Assert.AreEqual(true, jsonCollection.error);
        }

        [TestMethod()]
        public void Edit_Academic_Degree_View_Test()
        {
            // Arrange
            AcademicDegreeController controller = new AcademicDegreeController();
            academic_degree academicDegree = new academic_degree() { id = "1", name = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras consectetur sed tellus at tincidundsdss", level = 3 };
            mockSet.Setup(m => m.Find(It.IsAny<string>())).Returns(academicDegree);

            // Act
            ViewResult result;
            using (scope)
            {
                controller.Create(academicDegree);
                result = controller.Edit(academicDegree.id) as ViewResult;
            }

            // Assert
            Assert.IsNotNull(result);
        }
    }
}