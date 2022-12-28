using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL.Tests
{
    [TestClass()]
    public class TermRepositoryTests
    {
        private IQueryable<term> listTerm;
        private Mock<DbSet<term>> mockSet;
        private Mock<CP25Team03Entities> mockContext;
        private UnitOfWork unitOfWork;
        private TransactionScope scope;

        [TestInitialize()]
        public void Initialize()
        {
            listTerm = new List<term> {
                new term() { id = 123, start_year = 2022, end_year = 2023, start_week = 1, start_date = DateTime.Now, max_lesson = 6, max_class = 6 },
                new term() { id = 124, start_year = 2022, end_year = 2023, start_week = 1, start_date = DateTime.Now, max_lesson = 6, max_class = 6 }
            }.AsQueryable();
            mockSet = new Mock<DbSet<term>>();
            mockContext = new Mock<CP25Team03Entities>();
            unitOfWork = new UnitOfWork(mockContext.Object);
            scope = new TransactionScope();
            mockSet.As<IQueryable<term>>().Setup(m => m.Provider).Returns(listTerm.Provider);
            mockSet.As<IQueryable<term>>().Setup(m => m.Expression).Returns(listTerm.Expression);
            mockSet.As<IQueryable<term>>().Setup(m => m.ElementType).Returns(listTerm.ElementType);
            mockSet.As<IQueryable<term>>().Setup(m => m.GetEnumerator()).Returns(listTerm.GetEnumerator());
            mockContext.Setup(c => c.terms).Returns(() => mockSet.Object);
        }

        [TestCleanup()]
        public void TestCleanup()
        {
            unitOfWork.Dispose();
            scope.Dispose();
        }

        [TestMethod()]
        public void Get_Term_Json_Data_Not_Null_Test()
        {
            // Act
            IEnumerable<object> actionResult = unitOfWork.TermRepository.GetTerms().Cast<object>();

            // Assert
            Assert.AreEqual(actionResult.Count(), 2);
        }


        [TestMethod()]
        public void Insert_Term_Repository_Test()
        {
            // Arrange
            term term = new term() { id = 125, start_year = 2022, end_year = 2023, start_week = 1, start_date = DateTime.Now, max_lesson = 6, max_class = 6 };

            // Act
            unitOfWork.TermRepository.InsertTerm(term);
            unitOfWork.Save();

            // Assert
            mockSet.Verify(r => r.Add(term), Times.Once);
            mockContext.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod()]
        public void Delete_Term_Repository_Test()
        {
            // Arrange
            List<term> listTerm = new List<term> {
                new term() { id = 123, start_year = 2022, end_year = 2023, start_week = 1, start_date = DateTime.Now, max_lesson = 6, max_class = 6 },
                new term() { id = 124, start_year = 2022, end_year = 2023, start_week = 1, start_date = DateTime.Now, max_lesson = 6, max_class = 6 }
            };

            // Act
            mockSet.Setup(m => m.Remove(It.IsAny<term>())).Callback<term>((entity) => listTerm.Remove(entity));
            unitOfWork.TermRepository.DeleteTerm(listTerm[0].id);
            unitOfWork.Save();

            // Assert
            mockSet.Verify(x => x.Remove(It.IsAny<term>()), Times.Once());
            mockContext.Verify(r => r.SaveChanges(), Times.Once);
        }
    }
}