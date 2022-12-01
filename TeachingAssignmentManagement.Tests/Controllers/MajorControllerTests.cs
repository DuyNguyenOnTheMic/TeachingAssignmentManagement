using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
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
        public void Get_Major_Json_Data_Not_Null_Test()
        {
            // Arrange
            var data = new List<major> { new major() { id = "bla", name = "hehe" } }.AsQueryable();

            var mockSet = new Mock<DbSet<major>>();
            mockSet.As<IQueryable<major>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<major>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<major>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<major>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<CP25Team03Entities>();
            mockContext.Setup(c => c.majors).Returns(mockSet.Object);

            // Act
            var service = new UnitOfWork(mockContext.Object);
            var books = service.MajorRepository.GetMajors();
        }
    }
}