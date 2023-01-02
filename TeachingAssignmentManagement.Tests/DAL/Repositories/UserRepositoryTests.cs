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
    }
}