using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using TeachingAssignmentManagement.Controllers;

namespace TeachingAssignmentManagement.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}
