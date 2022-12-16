using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Controllers
{
    [Authorize(Roles = "BCN khoa, Bộ môn, Giảng viên")]
    public class StatisticsController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public StatisticsController()
        {
            unitOfWork = new UnitOfWork(new CP25Team03Entities());
        }

        public StatisticsController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public ActionResult Index()
        {
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            ViewData["year"] = new SelectList(unitOfWork.TermRepository.GetYears(), "schoolyear", "schoolyear");
            return View();
        }

        [HttpGet]
        public ActionResult GetChart(string type, string value, string lecturerType)
        {
            ViewData["type"] = type;
            ViewData["value"] = value;
            ViewData["lecturerType"] = lecturerType;
            return PartialView("_Chart");
        }

        [HttpGet]
        public JsonResult GetTermData(int termId, string lecturerType)
        {
            return Json(unitOfWork.CurriculumClassRepository.GetTermStatistics(termId, lecturerType), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetYearData(int startYear, int endYear, string lecturerType)
        {
            return Json(unitOfWork.CurriculumClassRepository.GetYearStatistics(startYear, endYear, lecturerType), JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}