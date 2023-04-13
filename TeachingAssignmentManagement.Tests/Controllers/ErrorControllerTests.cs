using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;

namespace TeachingAssignmentManagement.Controllers.Tests
{
    [TestClass()]
    public class ErrorControllerTests
    {
        [TestMethod()]
        public void Not_Found_View_Test()
        {
            // Arrange
            ErrorController controller = new ErrorController();

            // Act
            ViewResult result = controller.NotFound() as ViewResult;

            // Assert
            Assert.AreEqual("NotFound", result.ViewName);
        }
    }
}