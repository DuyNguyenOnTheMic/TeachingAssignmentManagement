using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL.Tests
{
    [TestClass()]
    public class UserRepositoryTests
    {
        private ICollection<AspNetRole> listRole;
        private IQueryable<AspNetUser> listUser;
        private IQueryable<lecturer> listLecturer;
        private Mock<DbSet<lecturer>> mockSetLecturer;
        private Mock<DbSet<AspNetUser>> mockSetUser;
        private Mock<CP25Team03Entities> mockContext;
        private UnitOfWork unitOfWork;
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
        }

        [TestMethod()]
        public void Get_User_Data_Not_Null_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.UserRepository.GetUsers();

            // Assert
            Assert.IsNotNull(actionResult);
        }

        [TestMethod()]
        public void Get_User_Data_Is_Correct_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.UserRepository.GetUsers();
            List<AspNetUser> userList = listUser.ToList();
            List<lecturer> lecturerList = listLecturer.ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            for (int i = 0; i < listUser.Count(); i++)
            {
                Assert.AreEqual(actionResult[i].id, userList[i].Id);
                Assert.AreEqual(actionResult[i].email, userList[i].Email);
                Assert.AreEqual(actionResult[i].role, userList[i].AspNetRoles.FirstOrDefault().Name);
                Assert.AreEqual(actionResult[i].staff_id, lecturerList[i].staff_id);
                Assert.AreEqual(actionResult[i].full_name, lecturerList[i].full_name);
                Assert.AreEqual(actionResult[i].type, lecturerList[i].type);
            }
        }

        [TestMethod()]
        public void User_Data_Should_Be_IEnumerable_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.UserRepository.GetUsers();
            int count = 0;
            foreach (dynamic value in actionResult)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod]
        public void User_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.UserRepository.GetUsers();

            // Assert                
            Assert.IsNotNull(actionResult[0]);
        }

        [TestMethod]
        public void User_Data_Should_Be_Indexable_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.UserRepository.GetUsers();

            // Assert
            for (int i = 0; i < actionResult.Count; i++)
            {

                dynamic json = actionResult[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.id);
                Assert.IsNotNull(json.email);
                Assert.IsNotNull(json.role);
                Assert.IsNotNull(json.staff_id);
                Assert.IsNotNull(json.full_name);
                Assert.IsNotNull(json.type);
            }
        }

        [TestMethod()]
        public void Get_User_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Act
            IEnumerable<object> actionResult = unitOfWork.UserRepository.GetUsers().Cast<object>();

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(listUser.Count(), actionResult.Count());
        }

        [TestMethod()]
        public void Get_User_Role_Data_Not_Null_Test()
        {
            // Arrange
            Mock<DbSet<AspNetRole>> mockSetRole = new Mock<DbSet<AspNetRole>>();
            mockContext.Setup(c => c.AspNetRoles).Returns(() => mockSetRole.Object);

            // Act
            IQueryable<AspNetRole> listRoles = new List<AspNetRole>(listRole).AsQueryable();
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.Provider).Returns(listRoles.Provider);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.Expression).Returns(listRoles.Expression);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.ElementType).Returns(listRoles.ElementType);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.GetEnumerator()).Returns(listRoles.GetEnumerator());

            dynamic actionResult = unitOfWork.UserRepository.GetRoles();

            // Assert
            Assert.IsNotNull(actionResult);
        }

        [TestMethod()]
        public void Get_User_Role_Data_Is_Correct_Test()
        {
            // Arrange
            Mock<DbSet<AspNetRole>> mockSetRole = new Mock<DbSet<AspNetRole>>();
            mockContext.Setup(c => c.AspNetRoles).Returns(() => mockSetRole.Object);

            // Act
            IQueryable<AspNetRole> listRoles = new List<AspNetRole>(listRole).AsQueryable();
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.Provider).Returns(listRoles.Provider);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.Expression).Returns(listRoles.Expression);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.ElementType).Returns(listRoles.ElementType);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.GetEnumerator()).Returns(listRoles.GetEnumerator());

            dynamic actionResult = unitOfWork.UserRepository.GetRoles().ToList();
            List<AspNetRole> roleList = listRole.ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            for (int i = 0; i < roleList.Count(); i++)
            {
                Assert.AreEqual(actionResult[i].Id, roleList[i].Id);
                Assert.AreEqual(actionResult[i].Name, roleList[i].Name);
            }
        }

        [TestMethod()]
        public void User_Role_Data_Should_Be_IEnumerable_Test()
        {
            // Arrange
            Mock<DbSet<AspNetRole>> mockSetRole = new Mock<DbSet<AspNetRole>>();
            mockContext.Setup(c => c.AspNetRoles).Returns(() => mockSetRole.Object);

            // Act
            IQueryable<AspNetRole> listRoles = new List<AspNetRole>(listRole).AsQueryable();
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.Provider).Returns(listRoles.Provider);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.Expression).Returns(listRoles.Expression);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.ElementType).Returns(listRoles.ElementType);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.GetEnumerator()).Returns(listRoles.GetEnumerator());

            dynamic actionResult = unitOfWork.UserRepository.GetRoles();
            int count = 0;
            foreach (dynamic value in actionResult)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod]
        public void User_Role_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Arrange
            Mock<DbSet<AspNetRole>> mockSetRole = new Mock<DbSet<AspNetRole>>();
            mockContext.Setup(c => c.AspNetRoles).Returns(() => mockSetRole.Object);

            // Act
            IQueryable<AspNetRole> listRoles = new List<AspNetRole>(listRole).AsQueryable();
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.Provider).Returns(listRoles.Provider);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.Expression).Returns(listRoles.Expression);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.ElementType).Returns(listRoles.ElementType);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.GetEnumerator()).Returns(listRoles.GetEnumerator());

            dynamic actionResult = unitOfWork.UserRepository.GetRoles().ToList();

            // Assert                
            Assert.IsNotNull(actionResult[0]);
        }

        [TestMethod]
        public void User_Role_Data_Should_Be_Indexable_Test()
        {
            // Arrange
            Mock<DbSet<AspNetRole>> mockSetRole = new Mock<DbSet<AspNetRole>>();
            mockContext.Setup(c => c.AspNetRoles).Returns(() => mockSetRole.Object);

            // Act
            IQueryable<AspNetRole> listRoles = new List<AspNetRole>(listRole).AsQueryable();
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.Provider).Returns(listRoles.Provider);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.Expression).Returns(listRoles.Expression);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.ElementType).Returns(listRoles.ElementType);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.GetEnumerator()).Returns(listRoles.GetEnumerator());

            dynamic actionResult = unitOfWork.UserRepository.GetRoles().ToList();

            // Assert
            for (int i = 0; i < actionResult.Count; i++)
            {

                dynamic json = actionResult[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.Id);
                Assert.IsNotNull(json.Name);
            }
        }

        [TestMethod()]
        public void Get_User_Role_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Arrange
            Mock<DbSet<AspNetRole>> mockSetRole = new Mock<DbSet<AspNetRole>>();
            mockContext.Setup(c => c.AspNetRoles).Returns(() => mockSetRole.Object);

            // Act
            IQueryable<AspNetRole> listRoles = new List<AspNetRole>(listRole).AsQueryable();
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.Provider).Returns(listRoles.Provider);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.Expression).Returns(listRoles.Expression);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.ElementType).Returns(listRoles.ElementType);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.GetEnumerator()).Returns(listRoles.GetEnumerator());

            IEnumerable<object> actionResult = unitOfWork.UserRepository.GetRoles().Cast<object>();

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(listRole.Count(), actionResult.Count());
        }

        [TestMethod]
        public void User_Role_Should_Order_By_Id_Test()
        {
            // Arrange
            Mock<DbSet<AspNetRole>> mockSetRole = new Mock<DbSet<AspNetRole>>();
            mockContext.Setup(c => c.AspNetRoles).Returns(() => mockSetRole.Object);

            // Act
            IQueryable<AspNetRole> listRoles = new List<AspNetRole>(listRole).AsQueryable();
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.Provider).Returns(listRoles.Provider);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.Expression).Returns(listRoles.Expression);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.ElementType).Returns(listRoles.ElementType);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.GetEnumerator()).Returns(listRoles.GetEnumerator());

            dynamic actionResult = unitOfWork.UserRepository.GetRoles().OrderBy(r => r.Id).ToList();
            List<AspNetRole> roleList = listRole.ToList();

            // Assert
            for (int i = 0; i < roleList.Count(); i++)
            {
                Assert.AreEqual(actionResult[i].Id, roleList[i].Id);
                Assert.AreEqual(actionResult[i].Name, roleList[i].Name);
            }
        }

        [TestMethod]
        public void Find_User_Role_Correctly_Test()
        {
            // Arrange
            AspNetRole role = listRole.First();
            Mock<DbSet<AspNetRole>> mockSetRole = new Mock<DbSet<AspNetRole>>();
            mockContext.Setup(c => c.AspNetRoles).Returns(() => mockSetRole.Object);
            mockSetRole.Setup(m => m.Find(It.IsAny<string>())).Returns(role);

            // Act
            IQueryable<AspNetRole> listRoles = new List<AspNetRole>(listRole).AsQueryable();
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.Provider).Returns(listRoles.Provider);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.Expression).Returns(listRoles.Expression);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.ElementType).Returns(listRoles.ElementType);
            mockSetRole.As<IQueryable<AspNetRole>>().Setup(m => m.GetEnumerator()).Returns(listRoles.GetEnumerator());

            AspNetRole actionResult = unitOfWork.UserRepository.GetRoleByID(role.Id);

            // Assert
            mockSetRole.Verify(x => x.Find(It.IsAny<string>()), Times.Once());
            Assert.AreEqual(role.Id, actionResult.Id);
            Assert.AreEqual(role.Name, actionResult.Name);
        }
    }
}