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
        private Mock<DbSet<lecturer>> mockSet;
        private Mock<DbSet<AspNetUser>> mockSetUser;
        private Mock<CP25Team03Entities> mockContext;
        private UnitOfWork unitOfWork;
        private TransactionScope scope;

        [TestInitialize()]
        public void Initialize()
        {
            string userId1 = Guid.NewGuid().ToString();
            string userId2 = Guid.NewGuid().ToString();
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
            mockSet = new Mock<DbSet<lecturer>>();
            mockSetUser = new Mock<DbSet<AspNetUser>>();
            mockContext = new Mock<CP25Team03Entities>();
            unitOfWork = new UnitOfWork(mockContext.Object);
            scope = new TransactionScope();
            mockSet.As<IQueryable<lecturer>>().Setup(m => m.Provider).Returns(listLecturer.Provider);
            mockSet.As<IQueryable<lecturer>>().Setup(m => m.Expression).Returns(listLecturer.Expression);
            mockSet.As<IQueryable<lecturer>>().Setup(m => m.ElementType).Returns(listLecturer.ElementType);
            mockSet.As<IQueryable<lecturer>>().Setup(m => m.GetEnumerator()).Returns(listLecturer.GetEnumerator());
            mockSetUser.As<IQueryable<AspNetUser>>().Setup(m => m.Provider).Returns(listUser.Provider);
            mockSetUser.As<IQueryable<AspNetUser>>().Setup(m => m.Expression).Returns(listUser.Expression);
            mockSetUser.As<IQueryable<AspNetUser>>().Setup(m => m.ElementType).Returns(listUser.ElementType);
            mockSetUser.As<IQueryable<AspNetUser>>().Setup(m => m.GetEnumerator()).Returns(listUser.GetEnumerator());
            mockContext.Setup(c => c.lecturers).Returns(() => mockSet.Object);
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
            foreach (var value in jsonCollection)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }
    }
}