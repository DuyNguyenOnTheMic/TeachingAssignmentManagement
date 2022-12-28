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
    public class TermControllerTests
    {
        private IQueryable<term> listTerm;
        private Mock<DbSet<term>> mockSet;
        private Mock<CP25Team03Entities> mockContext;
        private Mock<TermRepository> termRepo;
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
            termRepo = new Mock<TermRepository>(mockContext);
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
            TermController controller = new TermController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void Get_Term_Json_Data_Not_Null_Test()
        {
            // Arrange
            TermController controller = new TermController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.id);
            }
        }

        [TestMethod()]
        public void Get_Term_Json_Data_Is_Correct_Test()
        {
            // Arrange
            TermController controller = new TermController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;
            List<term> termList = listTerm.OrderByDescending(t => t.id).ToList();

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            for (int i = 0; i < listTerm.Count(); i++)
            {
                Assert.AreEqual(jsonCollection[i].id, termList[i].id);
                Assert.AreEqual(jsonCollection[i].start_year, termList[i].start_year);
            }
        }

        [TestMethod()]
        public void Get_Term_Json_Data_Not_False_Test()
        {
            // Arrange
            TermController controller = new TermController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetData();

            // Assert
            Assert.IsNotNull(actionResult, "No ActionResult returned from action method.");
            dynamic jsonCollection = actionResult.Data;
            foreach (dynamic json in jsonCollection)
            {
                Assert.IsNotNull(json.id,
                    "JSON record does not contain \"id\" required property.");
                Assert.IsNotNull(json.start_year,
                    "JSON record does not contain \"start_year\" required property.");
            }
        }

        [TestMethod()]
        public void Term_Json_Data_Should_Convert_To_IEnumerable_Test()
        {
            // Arrange
            TermController controller = new TermController(unitOfWork);

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
        public void Term_Json_Data_Index_at_0_Should_Not_Be_Null_Test()
        {
            // Arrange
            TermController controller = new TermController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;

            // Assert                
            Assert.IsNotNull(jsonCollection[0]);
        }

        [TestMethod]
        public void Term_JSon_Data_Should_Be_Indexable_Test()
        {
            // Arrange
            TermController controller = new TermController(unitOfWork);

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
                Assert.IsNotNull(json.start_year,
                    "JSON record does not contain \"start_year\" required property.");
            }
        }

        [TestMethod()]
        public void Get_Term_List_Should_Be_Not_Null_And_Equal_Test()
        {
            // Arrange
            TermController controller = new TermController(unitOfWork);

            // Act
            JsonResult actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(jsonCollection);
            Assert.AreEqual(listTerm.Count(), jsonCollection.Count);
        }

        [TestMethod()]
        public void Get_Term_List_Should_Order_By_Latest_Terms_Test()
        {
            // Arrange
            TermController controller = new TermController(unitOfWork);
            List<term> termList = listTerm.ToList();

            // Act
            JsonResult actionResult = controller.GetData();
            dynamic jsonCollection = actionResult.Data;

            // Assert
            Assert.IsNotNull(jsonCollection);
            Assert.AreEqual(jsonCollection[0].id, termList[1].id);
            Assert.AreEqual(jsonCollection[0].start_year, termList[1].start_year);
            Assert.AreEqual(jsonCollection[0].id, 124);
            Assert.AreEqual(jsonCollection[0].start_year, 2022);
        }

        [TestMethod()]
        public void Create_View_Test()
        {
            // Arrange
            TermController controller = new TermController();

            // Act
            ViewResult result = controller.Create() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void Create_Term_Test()
        {
            // Arrange
            TermController controller = new TermController(unitOfWork);
            term term = new term() { id = 125, start_year = 2022, end_year = 2023, start_week = 1, start_date = DateTime.Now, max_lesson = 6, max_class = 6 };

            // Act
            controller.Create(term);

            // Assert
            mockSet.Verify(r => r.Add(term), Times.Once);
            mockContext.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void Create_Should_Be_Failed_When_Term_Exists_Test()
        {
            // Arrange
            TermController controller = new TermController();
            term term = new term() { id = 125, start_year = 2022, end_year = 2023, start_week = 1, start_date = DateTime.Now, max_lesson = 6, max_class = 6 };

            // Act
            JsonResult result;
            dynamic jsonCollection;
            using (scope)
            {
                controller.Create(term);
                result = controller.Create(term) as JsonResult;
                jsonCollection = result.Data;
            }

            // Assert
            Assert.AreEqual(true, jsonCollection.error);
        }

        [TestMethod()]
        public void Edit_Term_View_Test()
        {
            // Arrange
            TermController controller = new TermController();
            term term = new term() { id = 125, start_year = 2022, end_year = 2023, start_week = 1, start_date = DateTime.Now, max_lesson = 6, max_class = 6 };

            // Act
            ViewResult result;
            using (scope)
            {
                controller.Create(term);
                result = controller.Edit(term.id) as ViewResult;
            }

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void Edit_Term_Data_Should_Load_Correctly_Test()
        {
            // Arrange
            TermController controller = new TermController();
            term term = new term() { id = 125, start_year = 2022, end_year = 2023, start_week = 1, start_date = DateTime.Now, max_lesson = 6, max_class = 6 };

            // Act
            ViewResult result;
            using (scope)
            {
                controller.Create(term);
                result = controller.Edit(term.id) as ViewResult;
            }
            term model = result.Model as term;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.AreEqual(model.id, term.id);
        }

        [TestMethod]
        public void Edit_Term_Mock_Test()
        {
            // Arrange
            TermController controller = new TermController(unitOfWork);
            term term = new term() { id = 125, start_year = 2022, end_year = 2023, start_week = 1, start_date = DateTime.Now, max_lesson = 6, max_class = 6 };

            // Act
            JsonResult result = controller.Edit(term) as JsonResult;
            dynamic jsonCollection = result.Data;

            // Assert
            Assert.AreEqual(true, jsonCollection.success);
            mockContext.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod()]
        public void Edit_Term_Data_Test()
        {
            // Arrange
            TermController controller = new TermController();
            unitOfWork = new UnitOfWork(new CP25Team03Entities());
            term term = new term() { id = 125, start_year = 2022, end_year = 2023, start_week = 1, start_date = DateTime.Now, max_lesson = 6, max_class = 6 };

            // Act
            term termEdited;
            using (scope)
            {
                controller.Create(term);
                term.start_year = 2019;
                term.end_year = 2020;
                controller.Edit(term);
                termEdited = unitOfWork.TermRepository.GetTermByID(term.id);
            }

            // Assert
            Assert.AreEqual(termEdited.start_year, 2019);
            Assert.AreEqual(termEdited.end_year, 2020);
        }

        [TestMethod]
        public void Delete_Term_Test()
        {
            // Arrange
            TermController controller = new TermController(unitOfWork);

            // Act
            JsonResult result = controller.Delete(listTerm.First().id) as JsonResult;
            dynamic jsonCollection = result.Data;

            // Assert
            Assert.IsNotNull(jsonCollection);
            mockSet.Verify(r => r.Remove(It.IsAny<term>()));
            mockContext.Verify(r => r.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public void Populate_Years_Not_Null_Test()
        {
            // Arrange
            TermController controller = new TermController(unitOfWork);
            int year = DateTime.Now.Year;

            // Act
            List<SelectListItem> result = controller.PopulateYears(year);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Populate_Years_List_Should_Be_Countable_Test()
        {
            // Arrange
            TermController controller = new TermController(unitOfWork);
            int year = DateTime.Now.Year;

            // Act
            List<SelectListItem> result = controller.PopulateYears(year);
            int count = 0;
            foreach (SelectListItem item in result)
            {
                count++;
            }

            // Assert
            Assert.IsTrue(count > 0);
        }

        [TestMethod]
        public void Populate_Years_List_Should_Start_Correctly_Test()
        {
            // Arrange
            TermController controller = new TermController(unitOfWork);
            int year = DateTime.Now.Year;
            int startYear = year - 10;

            // Act
            List<SelectListItem> result = controller.PopulateYears(year);

            // Assert
            Assert.AreEqual(startYear, int.Parse(result[0].Value));
        }

        [TestMethod]
        public void Populate_Years_List_Should_End_Correctly_Test()
        {
            // Arrange
            TermController controller = new TermController(unitOfWork);
            int year = DateTime.Now.Year;
            int endYear = year + 10;

            // Act
            List<SelectListItem> result = controller.PopulateYears(year);

            // Assert
            Assert.AreEqual(endYear, int.Parse(result.Last().Value));
        }

        [TestMethod]
        public void Populate_Years_List_Total_Count_Correctly_Test()
        {
            // Arrange
            TermController controller = new TermController(unitOfWork);
            int year = DateTime.Now.Year;

            // Act
            List<SelectListItem> result = controller.PopulateYears(year);

            // Assert
            Assert.AreEqual(21, result.Count);
        }

        #region RepositoryTests
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
            List<term>  listTerm = new List<term> {
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
        #endregion
    }
}