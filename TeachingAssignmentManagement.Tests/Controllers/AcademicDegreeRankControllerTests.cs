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
        private IQueryable<academic_degree_rank> listAcademicDegree;
        private Mock<DbSet<academic_degree_rank>> mockSet;
        private Mock<CP25Team03Entities> mockContext;
        private UnitOfWork unitOfWork;
        private TransactionScope scope;

        [TestInitialize()]
        public void Initialize()
        {
            listAcademicDegree = new List<academic_degree_rank> {
                new academic_degree_rank() { id = "1", academic_degree_id = "CN" },
                new academic_degree_rank() { id = "2", academic_degree_id = "THS" }
            }.AsQueryable();
            mockSet = new Mock<DbSet<academic_degree_rank>>();
            mockContext = new Mock<CP25Team03Entities>();
            unitOfWork = new UnitOfWork(mockContext.Object);
            scope = new TransactionScope();
            mockSet.As<IQueryable<academic_degree_rank>>().Setup(m => m.Provider).Returns(listAcademicDegree.Provider);
            mockSet.As<IQueryable<academic_degree_rank>>().Setup(m => m.Expression).Returns(listAcademicDegree.Expression);
            mockSet.As<IQueryable<academic_degree_rank>>().Setup(m => m.ElementType).Returns(listAcademicDegree.ElementType);
            mockSet.As<IQueryable<academic_degree_rank>>().Setup(m => m.GetEnumerator()).Returns(listAcademicDegree.GetEnumerator());
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
    }
}