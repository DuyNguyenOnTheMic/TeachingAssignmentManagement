using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
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
    public class UserControllerTests
    {
        private ICollection<AspNetRole> listRole;
        private IQueryable<AspNetUser> listUser;
        private IQueryable<lecturer> listLecturer;
        private Mock<DbSet<lecturer>> mockSetLecturer;
        private Mock<DbSet<AspNetUser>> mockSetUser;
        private Mock<CP25Team03Entities> mockContext;
        private UnitOfWork unitOfWork;
        private TransactionScope scope;
        readonly string userId1 = Guid.NewGuid().ToString();
        readonly string userId2 = Guid.NewGuid().ToString();

        [TestInitialize()]
        public void Initialize()
        {
            listRole = new List<AspNetRole> {
                new AspNetRole() { Id = "1", Name = "Giảng viên" }
            };
            listUser = new List<AspNetUser> {
                new AspNetUser() { Id = userId1, Email = "a.nv@vanlanguni.vn", UserName = "a.nv@vanlanguni.vn", AspNetRoles = listRole },
                new AspNetUser() { Id = userId2, Email = "b.nv@vlu.edu.vn", UserName = "b.nv@vlu.edu.vn", AspNetRoles = listRole }
            }.AsQueryable();
            listLecturer = new List<lecturer> {
                new lecturer() { id = userId1, staff_id = "1001", full_name = "Nguyễn Văn A", type = "TG" },
                new lecturer() { id = userId2, staff_id = "1002", full_name = "Nguyễn Văn B", type = "CH" }
            }.AsQueryable();
            mockSetLecturer = new Mock<DbSet<lecturer>>();
            mockSetUser = new Mock<DbSet<AspNetUser>>();
            mockContext = new Mock<CP25Team03Entities>();
            unitOfWork = new UnitOfWork(mockContext.Object);
            scope = new TransactionScope();
            mockSetUser.As<IQueryable<AspNetUser>>().Setup(m => m.Provider).Returns(listUser.Provider);
            mockSetUser.As<IQueryable<AspNetUser>>().Setup(m => m.Expression).Returns(listUser.Expression);
            mockSetUser.As<IQueryable<AspNetUser>>().Setup(m => m.ElementType).Returns(listUser.ElementType);
            mockSetUser.As<IQueryable<AspNetUser>>().Setup(m => m.GetEnumerator()).Returns(listUser.GetEnumerator());
            mockSetLecturer.As<IQueryable<lecturer>>().Setup(m => m.Provider).Returns(listLecturer.Provider);
            mockSetLecturer.As<IQueryable<lecturer>>().Setup(m => m.Expression).Returns(listLecturer.Expression);
            mockSetLecturer.As<IQueryable<lecturer>>().Setup(m => m.ElementType).Returns(listLecturer.ElementType);
            mockSetLecturer.As<IQueryable<lecturer>>().Setup(m => m.GetEnumerator()).Returns(listLecturer.GetEnumerator());
            mockContext.Setup(c => c.lecturers).Returns(() => mockSetLecturer.Object);
            mockContext.Setup(c => c.AspNetUsers).Returns(() => mockSetUser.Object);
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
            UserController controller = new UserController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void Get_User_Json_Data_Not_Null_Test()
        {
            // Arrange.
            UserController controller = new UserController(unitOfWork);

            // Act.
            JsonResult actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;

            // Assert.
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.id);
                Assert.IsNotNull(json.email);
                Assert.IsNotNull(json.role);
                Assert.IsNotNull(json.staff_id);
                Assert.IsNotNull(json.full_name);
                Assert.IsNotNull(json.type);
            }
        }

        [TestMethod()]
        public void Get_User_Json_Data_Correctly_Test()
        {
            // Arrange.
            UserController controller = new UserController(unitOfWork);

            // Act.
            JsonResult actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;

            // Assert.
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.id,
                    "JSON record does not contain \"id\" required property.");
                Assert.IsNotNull(json.email,
                    "JSON record does not contain \"email\" required property.");
                Assert.IsNotNull(json.role,
                    "JSON record does not contain \"role\" required property.");
                Assert.IsNotNull(json.staff_id,
                    "JSON record does not contain \"staff_id\" required property.");
                Assert.IsNotNull(json.full_name,
                    "JSON record does not contain \"full_name\" required property.");
                Assert.IsNotNull(json.type,
                    "JSON record does not contain \"type\" required property.");
            }
        }

        [TestMethod()]
        public void User_Json_Data_Should_Convert_To_IEnumerable_Test()
        {
            // Arrange
            UserController controller = new UserController();

            // Act
            JsonResult actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;
            int count = 0;
            foreach (dynamic value in jsonCollection)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod]
        public void User_Json_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Arrange
            UserController controller = new UserController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;

            // Assert                
            Assert.IsNotNull(jsonCollection[0]);
        }

        [TestMethod]
        public void User_JSon_Data_Should_Be_Indexable_Test()
        {
            // Arrange
            UserController controller = new UserController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;

            // Assert
            for (int i = 0; i < jsonCollection.Count; i++)
            {
                dynamic json = jsonCollection[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.id,
                    "JSON record does not contain \"id\" required property.");
                Assert.IsNotNull(json.email,
                    "JSON record does not contain \"email\" required property.");
                Assert.IsNotNull(json.role,
                    "JSON record does not contain \"role\" required property.");
                Assert.IsNotNull(json.staff_id,
                    "JSON record does not contain \"staff_id\" required property.");
                Assert.IsNotNull(json.full_name,
                    "JSON record does not contain \"full_name\" required property.");
                Assert.IsNotNull(json.type,
                    "JSON record does not contain \"type\" required property.");
            }
        }

        [TestMethod]
        public void User_JSon_Data_Count_Should_Be_Equal_Test()
        {
            // Arrange
            UserController controller = new UserController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.AreEqual(listUser.Count(), jsonCollection.Count);
        }

        [TestMethod()]
        public void Create_View_Test()
        {
            // Arrange
            UserController controller = new UserController();

            // Act
            ViewResult result = controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void Create_View_Should_Load_Role_SelectList_Test()
        {
            // Arrange
            UserController controller = new UserController(unitOfWork);
            Mock<DbSet<AspNetRole>> mockSetRole = new Mock<DbSet<AspNetRole>>();
            mockContext.Setup(c => c.AspNetRoles).Returns(() => mockSetRole.Object);

            // Act
            IQueryable<AspNetRole> listRoles = new List<AspNetRole>(listRole).AsQueryable();
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.Provider).Returns(listRoles.Provider);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.Expression).Returns(listRoles.Expression);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.ElementType).Returns(listRoles.ElementType);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.GetEnumerator()).Returns(listRoles.GetEnumerator());

            ViewResult result = controller.Create() as ViewResult;
            SelectList userRoles = new SelectList(listRole);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userRoles.Count(), ((IEnumerable<dynamic>)result.ViewBag.role_id).Count());
        }

        [TestMethod()]
        public void Return_Original_String_When_Not_Empty_Test()
        {
            // Arrange
            string test = "Nguyễn Văn A";

            // Act
            var result = UserController.SetNullOnEmpty(test);

            // Assert
            Assert.AreEqual(test, result);
        }

        [TestMethod()]
        public void Set_Null_On_Empty_String_Test()
        {
            // Arrange
            string test = "";

            // Act
            var result = UserController.SetNullOnEmpty(test);

            // Assert
            Assert.AreEqual(null, result);
        }

        [TestMethod()]
        public void Set_Null_On_Empty_String_Type_Test()
        {
            // Arrange
            string test = string.Empty;

            // Act
            var result = UserController.SetNullOnEmpty(test);

            // Assert
            Assert.AreEqual(null, result);
        }

        [TestMethod()]
        public void String_Should_Convert_To_Null_If_Have_Space_Test()
        {
            // Arrange
            string test = "      ";

            // Act
            var result = UserController.SetNullOnEmpty(test);

            // Assert
            Assert.AreEqual(null, result);
        }
    }
}