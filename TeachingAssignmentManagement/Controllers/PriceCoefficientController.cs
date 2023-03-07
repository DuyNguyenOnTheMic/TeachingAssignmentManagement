using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Helpers;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Controllers
{
    [Authorize(Roles = CustomRoles.FacultyBoard)]
    public class PriceCoefficientController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public PriceCoefficientController()
        {
            unitOfWork = new UnitOfWork(new CP25Team03Entities());
        }

        public PriceCoefficientController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public ActionResult Index()
        {
            ViewData["year"] = new SelectList(unitOfWork.TermRepository.GetYears(), "schoolyear", "schoolyear");
            return View();
        }

        [HttpGet]
        public ActionResult GetData(int startYear, int endYear)
        {
            // Get unit price and coefficient data from database
            IEnumerable<unit_price> query_unitPrice = unitOfWork.UnitPriceRepository.GetUnitPriceInYear(startYear, endYear);
            return PartialView("_PriceCoefficient", new PriceCoefficientViewModel
            {
                StartYear = startYear,
                EndYear = endYear,
                AcademicDegreeRankDTOs = unitOfWork.AcademicDegreeRankRepository.GetAcademicDegreeRankDTO(),
                StandardProgramDTOs = unitOfWork.UnitPriceRepository.GetUnitPriceByProgram(query_unitPrice, MyConstants.StandardProgramType),
                SpecialProgramDTOs = unitOfWork.UnitPriceRepository.GetUnitPriceByProgram(query_unitPrice, MyConstants.SpecialProgramType),
                ForeignDTOs = unitOfWork.UnitPriceRepository.GetUnitPriceByProgram(query_unitPrice, MyConstants.ForeignType),
                Coefficients = unitOfWork.CoefficientRepository.GetCoefficientInYear(startYear, endYear)
            });
        }

        [HttpGet]
        public ActionResult CreateUnitPrice(string rankId, int type, int startYear, int endYear)
        {
            ViewData["academic_degree_rank_id"] = rankId;
            ViewData["type"] = type;
            ViewData["start_year"] = startYear;
            ViewData["end_year"] = endYear;
            return View();
        }

        [HttpPost]
        public ActionResult CreateUnitPrice([Bind(Include = "type,start_year,end_year,academic_degree_rank_id")] unit_price unitPrice, string price)
        {
            // Create new unit price
            bool isUnitPriceExists = unitOfWork.UnitPriceRepository.CheckUnitPriceExists(unitPrice.type, unitPrice.start_year, unitPrice.end_year, unitPrice.academic_degree_rank_id);
            if (!isUnitPriceExists)
            {
                unitPrice.unit_price1 = decimal.Parse(price, CultureInfo.InvariantCulture);
                unitOfWork.UnitPriceRepository.InsertUnitPrice(unitPrice);
                unitOfWork.Save();
                return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { error = true, message = "Đơn giá cho cấp bậc đã được đặt trong năm học này, vui lòng thử lại sau!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditUnitPrice(int id)
        {
            return View(unitOfWork.UnitPriceRepository.GetUnitPriceByID(id));
        }

        [HttpPost]
        public ActionResult EditUnitPrice(int id, string price)
        {
            // Update unit price
            unit_price query_unitPrice = unitOfWork.UnitPriceRepository.GetUnitPriceByID(id);
            query_unitPrice.unit_price1 = decimal.Parse(price, CultureInfo.InvariantCulture);
            unitOfWork.Save();
            return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditAllUnitPrice(int type, int startYear, int endYear)
        {
            ViewData["type"] = type;
            ViewData["startYear"] = startYear;
            ViewData["endYear"] = endYear;
            return View();
        }

        [HttpPost]
        public ActionResult EditAllUnitPrice(int type, int startYear, int endYear, string price)
        {
            // Update all unit price
            unitOfWork.UnitPriceRepository.DeleteAllUnitPrice(type, startYear, endYear);
            IEnumerable<AcademicDegreeRankDTO> academicDegreeRankDTOs = unitOfWork.AcademicDegreeRankRepository.GetAcademicDegreeRankDTO();
            foreach (AcademicDegreeRankDTO item in academicDegreeRankDTOs)
            {
                unit_price unitPrice = new unit_price
                {
                    type = type,
                    unit_price1 = decimal.Parse(price, CultureInfo.InvariantCulture),
                    start_year = startYear,
                    end_year = endYear,
                    academic_degree_rank_id = item.Id
                };
                unitOfWork.UnitPriceRepository.InsertUnitPrice(unitPrice);
            }
            unitOfWork.Save();
            return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CreateCoefficient(int startYear, int endYear)
        {
            ViewData["start_year"] = startYear;
            ViewData["end_year"] = endYear;
            return View();
        }

        [HttpPost]
        public ActionResult CreateCoefficient([Bind(Include = "start_year,end_year")] coefficient coefficient, string vietnamese_coefficient, string foreign_coefficient, string theoretical_coefficient, string practice_coefficient)
        {
            // Create new coefficient
            bool isCoefficientExists = unitOfWork.CoefficientRepository.CheckCoefficientExists(coefficient.start_year, coefficient.end_year);
            if (!isCoefficientExists)
            {
                coefficient.vietnamese_coefficient = decimal.Parse(vietnamese_coefficient, CultureInfo.InvariantCulture);
                coefficient.foreign_coefficient = decimal.Parse(foreign_coefficient, CultureInfo.InvariantCulture);
                coefficient.theoretical_coefficient = decimal.Parse(theoretical_coefficient, CultureInfo.InvariantCulture);
                coefficient.practice_coefficient = decimal.Parse(practice_coefficient, CultureInfo.InvariantCulture);
                unitOfWork.CoefficientRepository.InsertCoefficient(coefficient);
                unitOfWork.Save();
                return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { error = true, message = "Hệ số cho cấp bậc đã được đặt trong năm học này, vui lòng thử lại sau!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditCoefficient(int id)
        {
            return View(unitOfWork.CoefficientRepository.GetCoefficientByID(id));
        }

        [HttpPost]
        public ActionResult EditCoefficient(int id, string vietnamese_coefficient, string foreign_coefficient, string theoretical_coefficient, string practice_coefficient)
        {
            // Update coefficient
            coefficient query_coefficient = unitOfWork.CoefficientRepository.GetCoefficientByID(id);
            query_coefficient.vietnamese_coefficient = decimal.Parse(vietnamese_coefficient, CultureInfo.InvariantCulture);
            query_coefficient.foreign_coefficient = decimal.Parse(foreign_coefficient, CultureInfo.InvariantCulture);
            query_coefficient.theoretical_coefficient = decimal.Parse(theoretical_coefficient, CultureInfo.InvariantCulture);
            query_coefficient.practice_coefficient = decimal.Parse(practice_coefficient, CultureInfo.InvariantCulture);
            unitOfWork.Save();
            return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}