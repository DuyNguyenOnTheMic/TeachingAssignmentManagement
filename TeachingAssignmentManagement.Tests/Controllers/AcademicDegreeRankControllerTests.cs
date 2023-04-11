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
    public class AcademicDegreeRankControllerTests
    {
        private IQueryable<academic_degree> listAcademicDegree;
        private IQueryable<academic_degree_rank> listAcademicDegreeRank;
        private Mock<DbSet<academic_degree_rank>> mockSet;
        private Mock<CP25Team03Entities> mockContext;
        private UnitOfWork unitOfWork;
        private TransactionScope scope;

        [TestInitialize()]
        public void Initialize()
        {
            listAcademicDegree = new List<academic_degree> {
                new academic_degree() { id = "CN", name = "Cử nhân", level = 5 },
                new academic_degree() { id = "THS", name = "Thạc sĩ", level = 4 }
            }.AsQueryable();
            listAcademicDegreeRank = new List<academic_degree_rank> {
                new academic_degree_rank() { id = "1", academic_degree = listAcademicDegree.First() },
                new academic_degree_rank() { id = "2", academic_degree = listAcademicDegree.Last() }
            }.AsQueryable();
            mockSet = new Mock<DbSet<academic_degree_rank>>();
            mockContext = new Mock<CP25Team03Entities>();
            unitOfWork = new UnitOfWork(mockContext.Object);
            scope = new TransactionScope();
            mockSet.As<IQueryable<academic_degree_rank>>().Setup(m => m.Provider).Returns(listAcademicDegreeRank.Provider);
            mockSet.As<IQueryable<academic_degree_rank>>().Setup(m => m.Expression).Returns(listAcademicDegreeRank.Expression);
            mockSet.As<IQueryable<academic_degree_rank>>().Setup(m => m.ElementType).Returns(listAcademicDegreeRank.ElementType);
            mockSet.As<IQueryable<academic_degree_rank>>().Setup(m => m.GetEnumerator()).Returns(listAcademicDegreeRank.GetEnumerator());
            mockContext.Setup(c => c.academic_degree_rank).Returns(() => mockSet.Object);
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
            AcademicDegreeRankController controller = new AcademicDegreeRankController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void Get_Academic_Degree_Rank_Json_Data_Not_Null_Test()
        {
            // Arrange
            AcademicDegreeRankController controller = new AcademicDegreeRankController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.id);
                Assert.IsNotNull(json.group);
            }
        }

        [TestMethod()]
        public void Get_Academic_Degree_Rank_Json_Data_Is_Correct_Test()
        {
            // Arrange
            AcademicDegreeRankController controller = new AcademicDegreeRankController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;
            List<academic_degree_rank> academicDegreeRankList = listAcademicDegreeRank.ToList();

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            for (int i = 0; i < listAcademicDegree.Count(); i++)
            {
                Assert.AreEqual(jsonCollection[i].id, academicDegreeRankList[i].id);
                Assert.AreEqual(jsonCollection[i].group, academicDegreeRankList[i].academic_degree.level + ". " + academicDegreeRankList[i].academic_degree_id + " (" + academicDegreeRankList[i].academic_degree.name + ")");
            }
        }

        [TestMethod()]
        public void Get_Academic_Degree_Rank_Json_Data_Not_False_Test()
        {
            // Arrange
            AcademicDegreeRankController controller = new AcademicDegreeRankController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetData();

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            dynamic jsonCollection = actionResult.Data;
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.id,
                    "JSON record does not contain \"id\" required property.");
                Assert.IsNotNull(json.group,
                    "JSON record does not contain \"group\" required property.");
            }
        }

        [TestMethod()]
        public void Academic_Degree_Rank_Json_Data_Should_Convert_To_IEnumerable_Test()
        {
            // Arrange
            AcademicDegreeRankController controller = new AcademicDegreeRankController(unitOfWork);

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
    }
}