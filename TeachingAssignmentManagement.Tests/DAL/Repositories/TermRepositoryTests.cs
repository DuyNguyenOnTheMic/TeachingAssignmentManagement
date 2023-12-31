﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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

        [TestInitialize()]
        public void Initialize()
        {
            listTerm = new List<term> {
                new term() { id = 123, start_year = 2022, end_year = 2023, start_week = 1, start_date = DateTime.Now, max_lesson = 6, max_class = 6 },
                new term() { id = 124, start_year = 2023, end_year = 2024, start_week = 1, start_date = DateTime.Now, max_lesson = 6, max_class = 6 }
            }.AsQueryable();
            mockSet = new Mock<DbSet<term>>();
            mockContext = new Mock<CP25Team03Entities>();
            unitOfWork = new UnitOfWork(mockContext.Object);
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
        }

        [TestMethod()]
        public void Get_Term_Data_Not_Null_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.TermRepository.GetTerms();

            // Assert
            Assert.IsNotNull(actionResult);
        }

        [TestMethod()]
        public void Get_Term_Data_Is_Correct_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.TermRepository.GetTerms();
            List<term> termList = listTerm.OrderByDescending(t => t.id).ToList();

            // Assert
            Assert.IsNotNull(actionResult);
            for (int i = 0; i < listTerm.Count(); i++)
            {
                Assert.AreEqual(actionResult[i].id, termList[i].id);
                Assert.AreEqual(actionResult[i].start_year, termList[i].start_year);
            }
        }

        [TestMethod()]
        public void Term_Data_Should_Be_IEnumerable_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.TermRepository.GetTerms();
            int count = 0;
            foreach (dynamic value in actionResult)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Term_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.TermRepository.GetTerms();

            // Assert                
            Assert.IsNotNull(actionResult[0]);
        }

        [TestMethod()]
        public void Term_Data_Should_Be_Indexable_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.TermRepository.GetTerms();

            // Assert
            for (int i = 0; i < actionResult.Count; i++)
            {

                dynamic json = actionResult[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.id);
                Assert.IsNotNull(json.start_year);
            }
        }

        [TestMethod()]
        public void Get_Term_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Act
            IEnumerable<object> actionResult = unitOfWork.TermRepository.GetTerms().Cast<object>();

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(listTerm.Count(), actionResult.Count());
        }

        [TestMethod()]
        public void Get_Term_Should_Order_By_Latest_Terms_Test()
        {
            // Arrange
            List<term> termList = listTerm.ToList();

            // Act
            dynamic actionResult = unitOfWork.TermRepository.GetTerms();

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(actionResult[0].id, termList[1].id);
            Assert.AreEqual(actionResult[0].id, 124);
        }

        [TestMethod()]
        public void Get_Year_Data_Not_Null_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.TermRepository.GetYears();

            // Assert
            Assert.IsNotNull(actionResult);
        }

        [TestMethod()]
        public void Get_Year_Data_Is_Correct_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.TermRepository.GetYears();
            term term = listTerm.First();
            string schoolyear = term.start_year + " - " + term.end_year;

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(actionResult[1].schoolyear, schoolyear);
        }

        [TestMethod()]
        public void Year_Data_Should_Be_IEnumerable_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.TermRepository.GetYears();
            int count = 0;
            foreach (dynamic value in actionResult)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod()]
        public void Year_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.TermRepository.GetYears();

            // Assert                
            Assert.IsNotNull(actionResult[0]);
        }

        [TestMethod()]
        public void Year_Data_Should_Be_Indexable_Test()
        {
            // Act
            dynamic actionResult = unitOfWork.TermRepository.GetYears();

            // Assert
            for (int i = 0; i < actionResult.Count; i++)
            {

                dynamic json = actionResult[i];

                Assert.IsNotNull(json);
                Assert.IsNotNull(json.schoolyear);
            }
        }

        [TestMethod()]
        public void Get_Year_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Act
            IEnumerable<object> actionResult = unitOfWork.TermRepository.GetYears().Cast<object>();

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(listTerm.Count(), actionResult.Count());
        }

        [TestMethod()]
        public void Get_Year_Should_Order_By_Latest_Years_Test()
        {
            // Arrange
            List<term> termList = listTerm.ToList();
            string yearBefore = termList[1].start_year + " - " + termList[1].end_year;
            string yearAfter = termList[0].start_year + " - " + termList[0].end_year;

            // Act
            dynamic actionResult = unitOfWork.TermRepository.GetYears();

            // Assert
            Assert.IsNotNull(actionResult);
            Assert.AreEqual(actionResult[0].schoolyear, yearBefore);
            Assert.AreEqual(actionResult[1].schoolyear, yearAfter);
        }

        [TestMethod()]
        public void Find_Term_Correctly_Test()
        {
            // Arrange
            term term = listTerm.First();
            mockSet.Setup(m => m.Find(It.IsAny<int>())).Returns(term);

            // Act
            term actionResult = unitOfWork.TermRepository.GetTermByID(term.id);

            // Assert
            mockSet.Verify(x => x.Find(It.IsAny<int>()), Times.Once());
            Assert.AreEqual(term.id, actionResult.id);
            Assert.AreEqual(term.start_year, actionResult.start_year);
            Assert.AreEqual(term.end_year, actionResult.end_year);
            Assert.AreEqual(term.start_week, actionResult.start_week);
            Assert.AreEqual(term.start_date, actionResult.start_date);
            Assert.AreEqual(term.max_lesson, actionResult.max_lesson);
            Assert.AreEqual(term.max_class, actionResult.max_class);
        }

        [TestMethod()]
        public void Insert_Term_Test()
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
            // Act
            unitOfWork.TermRepository.DeleteTerm(listTerm.FirstOrDefault().id);
            unitOfWork.Save();

            // Assert
            mockSet.Verify(x => x.Remove(It.IsAny<term>()), Times.Once());
            mockContext.Verify(r => r.SaveChanges(), Times.Once);
        }
    }
}