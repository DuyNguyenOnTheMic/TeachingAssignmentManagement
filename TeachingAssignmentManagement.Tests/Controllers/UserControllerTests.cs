using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using System;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Controllers.Tests
{
    [TestClass()]
    public class UserControllerTests
    {
        private IQueryable<lecturer> listLecturer;
        private Mock<DbSet<lecturer>> mockSet;
        private Mock<CP25Team03Entities> mockContext;
        private UnitOfWork unitOfWork;
        private TransactionScope scope;

        [TestInitialize()]
        public void Initialize()
        {
            listLecturer = new List<lecturer> {
                new lecturer() { id = "1", staff_id = "1001", full_name = "Nguyễn Văn A", type = "TG" },
                new lecturer() { id = "2", staff_id = "1002", full_name = "Nguyễn Văn B", type = "CH" }
            }.AsQueryable();
            mockSet = new Mock<DbSet<lecturer>>();
            mockContext = new Mock<CP25Team03Entities>();
            unitOfWork = new UnitOfWork(mockContext.Object);
            scope = new TransactionScope();
            mockSet.As<IQueryable<lecturer>>().Setup(m => m.Provider).Returns(listLecturer.Provider);
            mockSet.As<IQueryable<lecturer>>().Setup(m => m.Expression).Returns(listLecturer.Expression);
            mockSet.As<IQueryable<lecturer>>().Setup(m => m.ElementType).Returns(listLecturer.ElementType);
            mockSet.As<IQueryable<lecturer>>().Setup(m => m.GetEnumerator()).Returns(listLecturer.GetEnumerator());
            mockContext.Setup(c => c.lecturers).Returns(() => mockSet.Object);
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            unitOfWork.Dispose();
            scope.Dispose();
        }
    }
}