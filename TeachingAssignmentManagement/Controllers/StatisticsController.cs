using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [Authorize(Roles = "BCN khoa, Bộ môn")]
        public ActionResult Index()
        {
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            ViewData["year"] = new SelectList(unitOfWork.TermRepository.GetYears(), "schoolyear", "schoolyear");
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "BCN khoa, Bộ môn")]
        public ActionResult GetChart(string type, string value, string lecturerType)
        {
            ViewData["type"] = type;
            ViewData["value"] = value;
            ViewData["lecturerType"] = lecturerType;
            return PartialView("_Chart");
        }

        [HttpGet]
        [Authorize(Roles = "BCN khoa, Bộ môn")]
        public JsonResult GetTermData(int termId, string lecturerType)
        {
            IEnumerable query_classes = lecturerType != "-1"
                ? unitOfWork.CurriculumClassRepository.GetTermStatistics(termId, lecturerType)
                : unitOfWork.CurriculumClassRepository.GetTermStatisticsAll(termId);
            return Json(query_classes, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "BCN khoa, Bộ môn")]
        public ActionResult GetTermCurriculums(int termId, string lecturerId)
        {
            return Json(unitOfWork.CurriculumClassRepository.GetTermCurriculums(termId, lecturerId), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "BCN khoa, Bộ môn")]
        public JsonResult GetYearData(int startYear, int endYear, string lecturerType)
        {
            IEnumerable query_classes = lecturerType != "-1"
                ? unitOfWork.CurriculumClassRepository.GetYearStatistics(startYear, endYear, lecturerType)
                : unitOfWork.CurriculumClassRepository.GetYearStatisticsAll(startYear, endYear);
            return Json(query_classes, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "BCN khoa, Bộ môn")]
        public ActionResult GetYearCurriculums(int startYear, int endYear, string lecturerId)
        {
            return Json(unitOfWork.CurriculumClassRepository.GetYearCurriculums(startYear, endYear, lecturerId), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Timetable()
        {
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            return View();
        }

        [HttpGet]
        public ActionResult GetTimetable(int termId)
        {
            IEnumerable<CurriculumClassDTO> query_classes = unitOfWork.CurriculumClassRepository.GetTermAssignTimetable(termId);
            ViewBag.curriculums = unitOfWork.CurriculumRepository.GetCurriculums(query_classes);
            ViewBag.lecturers = new SelectList(unitOfWork.UserRepository.GetLecturers(), "id", "full_name");
            return PartialView("_Timetable", new TimetableViewModels
            {
                CurriculumClassDTOs = query_classes.ToList()
            });
        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}