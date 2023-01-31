using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;

namespace TeachingAssignmentManagement.Controllers.Tests
{
    [TestClass()]
    public class TimetableControllerTests
    {
        [TestMethod()]
        public void Create_View_Test()
        {
            // Arrange
            TimetableController controller = new TimetableController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}