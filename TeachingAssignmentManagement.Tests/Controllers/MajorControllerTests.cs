using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
        public void Insert_Major_Repository_Test()
        {
            // Arrange
            var major = new major() { id = "bla", name = "hehe" };
            var mockSet = new Mock<DbSet<major>>();
            var mockContext = new Mock<CP25Team03Entities>();
            mockContext.Setup(c => c.majors).Returns(mockSet.Object);

            // Act
            var service = new UnitOfWork(mockContext.Object);
            service.MajorRepository.InsertMajor(major);
            service.Save();

            //Act
            mockSet.Verify(r => r.Add(major), Times.Once);
            mockContext.Verify(r => r.SaveChanges(), Times.Once);
        }
    }
}