using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
            bool ajaxRequest = false;
            Mock<ControllerContext> fakeContext = new Mock<ControllerContext>();
            fakeContext.Setup(r => r.HttpContext.Request["X-Requested-With"])
                .Returns(ajaxRequest ? "XMLHttpRequest" : string.Empty);
            controller.ControllerContext = fakeContext.Object;

            // Act
            ViewResult result = controller.NotFound() as ViewResult;

            // Assert
            Assert.AreEqual("NotFound", result.ViewName);
        }
    }
}