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
    }
}