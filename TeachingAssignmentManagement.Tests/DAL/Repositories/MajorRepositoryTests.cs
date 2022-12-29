using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL.Tests
{
    [TestClass()]
    public class MajorRepositoryTests
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
                new major() { id = "1", name = "Công Nghệ Thông Tin", abbreviation = "CNTT" },
                new major() { id = "2", name = "Kỹ Thuật Phần Mềm", abbreviation = "KTPM" }
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

    }
}