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
        public ActionResult GetChart()
        {
            return PartialView("_Chart");
        }

        [HttpGet]
        public ActionResult GetCurriculums(string lecturerId)
        {
            var lecturer = unitOfWork.UserRepository.GetLecturerByID(lecturerId);
            ViewData["lecturerId"] = lecturerId;
            ViewData["lecturerName"] = lecturer.full_name;
            return PartialView("_Curriculums");
        }

        [HttpGet]
        public JsonResult GetTermData(int termId)
        {
            return Json(unitOfWork.CurriculumClassRepository.GetTermStatistics(termId), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTermCurriculums(int termId, string lecturerId)
        {
            return Json(unitOfWork.CurriculumClassRepository.GetTermCurriculums(termId, lecturerId), JsonRequestBehavior.AllowGet);
        }

    }
}