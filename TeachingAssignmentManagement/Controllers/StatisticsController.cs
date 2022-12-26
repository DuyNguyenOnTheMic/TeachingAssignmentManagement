using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
            ViewData["major"] = new SelectList(unitOfWork.MajorRepository.GetMajors(), "id", "name");
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "BCN khoa, Bộ môn")]
        public ActionResult GetChart(bool isLesson, string type, string value, string major, string lecturerType)
        {
            ViewData["isLesson"] = isLesson;
            ViewData["type"] = type;
            ViewData["value"] = value;
            ViewData["major"] = major;
            ViewData["majorAbb"] = major != "-1" ? unitOfWork.MajorRepository.GetMajorByID(major).abbreviation : "tất cả";
            ViewData["lecturerType"] = lecturerType;
            return PartialView("_Chart");
        }

        [HttpGet]
        [Authorize(Roles = "BCN khoa, Bộ môn")]
        public JsonResult GetTermData(bool isLesson, int termId, string majorId, string lecturerType)
        {
            IEnumerable query_classes = lecturerType != "-1"
                ? unitOfWork.CurriculumClassRepository.GetTermStatistics(isLesson, termId, majorId, lecturerType)
                : unitOfWork.CurriculumClassRepository.GetTermStatisticsAll(isLesson, termId, majorId);
            return Json(query_classes, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "BCN khoa, Bộ môn")]
        public ActionResult GetTermCurriculums(int termId, string majorId, string lecturerId)
        {
            return Json(unitOfWork.CurriculumClassRepository.GetTermCurriculums(termId, majorId, lecturerId), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "BCN khoa, Bộ môn")]
        public JsonResult GetYearData(int startYear, int endYear, string majorId, string lecturerType)
        {
            IEnumerable query_classes = lecturerType != "-1"
                ? unitOfWork.CurriculumClassRepository.GetYearStatistics(startYear, endYear, majorId, lecturerType)
                : unitOfWork.CurriculumClassRepository.GetYearStatisticsAll(startYear, endYear, majorId);
            return Json(query_classes, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "BCN khoa, Bộ môn")]
        public ActionResult GetYearCurriculums(int startYear, int endYear, string majorId, string lecturerId)
        {
            return Json(unitOfWork.CurriculumClassRepository.GetYearCurriculums(startYear, endYear, majorId, lecturerId), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Timetable()
        {
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            return View();
        }

        [HttpGet]
        public ActionResult GetTimetable(int termId, int week)
        {
            // Declare variables
            term term = unitOfWork.TermRepository.GetTermByID(termId);
            IEnumerable<CurriculumClassDTO> query_classes = unitOfWork.CurriculumClassRepository.GetTimetableStatistics(termId);

            // Populate timetable
            DateTime startDate = term.start_date;
            DateTime endDate = new DateTime();
            int startWeek = term.start_week;
            int endWeek = query_classes.Max(c => c.EndWeek);
            int currentWeek = 0;
            string weekLabel = string.Empty;

            if (week > 0)
            {
                // Set week based on user's selection
                startDate = startDate.AddDays((week - startWeek) * 7);
                endDate = startDate.AddDays(6);
                currentWeek = week;
            }
            else
            {
                // Get current week
                for (int i = startWeek; i <= endWeek; i++)
                {
                    endDate = startDate.AddDays(6);
                    for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                    {
                        if (date == DateTime.Today)
                        {
                            currentWeek = i;
                            break;
                        }
                    }
                    if (currentWeek == 0)
                    {
                        startDate = endDate.AddDays(1);
                    }
                    else
                    {
                        break;
                    }
                }
                if (currentWeek == 0)
                {
                    if (term.start_date < DateTime.Today)
                    {
                        // Set current week in case start date is in the past
                        startDate = startDate.AddDays(-7);
                        currentWeek = endWeek;
                    }
                    else
                    {
                        // Set current week in case start date is in the future
                        startDate = term.start_date;
                        endDate = startDate.AddDays(6);
                        currentWeek = startWeek;
                    }
                }
            }
            // Get current user language date format
            string[] userLang = Request.UserLanguages;
            string language = userLang[0];
            string formatInfo = new CultureInfo(language).DateTimeFormat.ShortDatePattern;
            weekLabel = "Tuần " + currentWeek + ": Từ ngày " + startDate.ToString(formatInfo) + " đến ngày " + endDate.ToString(formatInfo);

            ViewData["startWeek"] = startWeek;
            ViewData["endWeek"] = endWeek;
            ViewData["currentWeek"] = currentWeek;
            ViewData["weekLabel"] = weekLabel;
            ViewBag.curriculums = unitOfWork.CurriculumRepository.GetCurriculums(query_classes);
            ViewBag.lecturers = new SelectList(unitOfWork.UserRepository.GetLecturers(), "id", "full_name");
            return PartialView("_Timetable", new TimetableViewModels
            {
                CurriculumClassDTOs = unitOfWork.CurriculumClassRepository.GetClassInWeek(query_classes, currentWeek).ToList()
            });
        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}