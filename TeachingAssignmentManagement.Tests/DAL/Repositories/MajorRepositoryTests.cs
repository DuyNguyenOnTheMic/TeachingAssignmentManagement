using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.UI.WebControls;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL.Tests
{
    [TestClass()]
    public class MajorRepositoryTests
    {
        private IQueryable<major> listMajor;
        private Mock<DbSet<major>> mockSet;
        private Mock<CP25Team03Entities> mockContext;
        private UnitOfWork unitOfWork;

        [TestInitialize()]
        public void Initialize()
        {
            listMajor = new List<major> {
                new major() { id = "1", name = "Công Nghệ Thông Tin", abbreviation = "CNTT" },
                new major() { id = "2", name = "Kỹ Thuật Phần Mềm", abbreviation = "KTPM" }
            }.AsQueryable();
            mockSet = new Mock<DbSet<major>>();
            mockContext = new Mock<CP25Team03Entities>();
            unitOfWork = new UnitOfWork(mockContext.Object);
            mockSet.As<IQueryable<major>>().Setup(m => m.Provider).Returns(listMajor.Provider);
            mockSet.As<IQueryable<major>>().Setup(m => m.Expression).Returns(listMajor.Expression);
            mockSet.As<IQueryable<major>>().Setup(m => m.ElementType).Returns(listMajor.ElementType);
            mockSet.As<IQueryable<major>>().Setup(m => m.GetEnumerator()).Returns(listMajor.GetEnumerator());
            mockContext.Setup(c => c.majors).Returns(() => mockSet.Object);
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            unitOfWork.Dispose();
        }

        [TestMethod()]
        public void Get_Major_Data_Not_Null_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.MajorRepository.GetMajors();

            // Assert
            Assert.IsNotNull(actionResult);
        }

        [TestMethod()]
        public void Get_Major_Data_Is_Correct_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.MajorRepository.GetMajors();
            List<major> majorList = listMajor.OrderByDescending(m => m.name.Contains("công nghệ thông tin")).ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            for (int i = 0; i < listMajor.Count(); i++)
            {
                Assert.AreEqual(actionResult[i].id, majorList[i].id);
                Assert.AreEqual(actionResult[i].name, majorList[i].name);
                Assert.AreEqual(actionResult[i].abbreviation, majorList[i].abbreviation);
            }
        }

        [TestMethod()]
        public void Major_Data_Should_Be_IEnumerable_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.MajorRepository.GetMajors();
            int count = 0;
            foreach (dynamic value in actionResult)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Major_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.MajorRepository.GetMajors();

            // Assert                
            Assert.IsNotNull(actionResult[0]);
        }

        [TestMethod()]
        public void Major_Data_Should_Be_Indexable_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.MajorRepository.GetMajors();

            // Assert
            for (int i = 0; i < actionResult.Count; i++)
            {

                dynamic json = actionResult[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.name);
                Assert.IsNotNull(json.abbreviation);
            }
        }

        [TestMethod()]
        public void Get_Major_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Act
            IEnumerable<object> actionResult = unitOfWork.MajorRepository.GetMajors().Cast<object>();

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(listMajor.Count(), actionResult.Count());
        }

        [TestMethod()]
        public void Get_IT_Major_Should_Be_On_Top_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.MajorRepository.GetMajors();

            // Assert
            Assert.AreEqual(actionResult[0].id, "1");
            Assert.AreEqual(actionResult[0].name, "Công Nghệ Thông Tin");
        }

        [TestMethod()]
        public void Find_Major_Correctly_Test()
        {
            // Arrange
            major major = listMajor.First();
            mockSet.Setup(m => m.Find(It.IsAny<string>())).Returns(major);

            // Act
            major actionResult = unitOfWork.MajorRepository.GetMajorByID(major.id);

            // Assert
            mockSet.Verify(x => x.Find(It.IsAny<string>()), Times.Once());
            Assert.AreEqual(major.id, actionResult.id);
            Assert.AreEqual(major.name, actionResult.name);
            Assert.AreEqual(major.abbreviation, actionResult.abbreviation);
        }

        [TestMethod()]
        public void Insert_Major_Repository_Test()
        {
            // Arrange
            major major = new major() { id = "122", name = "hehe" };

            // Act
            unitOfWork.MajorRepository.InsertMajor(major);
            unitOfWork.Save();

            // Assert
            mockSet.Verify(r => r.Add(major), Times.Once);
            mockContext.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod()]
        public void Delete_Major_Repository_Test()
        {
            // Act
            unitOfWork.MajorRepository.DeleteMajor(listMajor.FirstOrDefault().id);
            unitOfWork.Save();

            // Assert
            mockSet.Verify(x => x.Remove(It.IsAny<major>()), Times.Once());
            mockContext.Verify(r => r.SaveChanges(), Times.Once);
        }
    }
}