using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using System.Web.UI.WebControls;
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

        [TestMethod()]
        public void Get_Major_Data_Not_Null_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.MajorRepository.GetMajors();

            // Assert
            Assert.IsNotNull(actionResult);
        }

        [TestMethod()]
        public void Get_Major_Data_Is_Correct_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.MajorRepository.GetMajors();
            List<major> majorList = listMajor.OrderByDescending(m => m.name.Contains("công nghệ thông tin")).ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            for (int i = 0; i < listMajor.Count(); i++)
            {
                Assert.AreEqual(actionResult[i].id, majorList[i].id);
                Assert.AreEqual(actionResult[i].name, majorList[i].name);
                Assert.AreEqual(actionResult[i].abbreviation, majorList[i].abbreviation);
            }
        }
    }
}