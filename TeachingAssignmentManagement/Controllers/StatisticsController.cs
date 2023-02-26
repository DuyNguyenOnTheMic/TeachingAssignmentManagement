using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Controllers
{
    [Authorize(Roles = "BCN khoa, Bộ môn, Giảng viên")]
    public class StatisticsController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly UnitOfWork unitOfWork;

        public StatisticsController()
        {
            unitOfWork = new UnitOfWork(new CP25Team03Entities());
        }

        public StatisticsController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
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
                ? unitOfWork.ClassSectionRepository.GetTermStatistics(isLesson, termId, majorId, lecturerType)
                : unitOfWork.ClassSectionRepository.GetTermStatisticsAll(isLesson, termId, majorId);
            return Json(query_classes, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "BCN khoa, Bộ môn")]
        public ActionResult GetTermSubjects(int termId, string majorId, string lecturerId)
        {
            return Json(unitOfWork.ClassSectionRepository.GetTermSubjects(termId, majorId, lecturerId), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "BCN khoa, Bộ môn")]
        public JsonResult GetYearData(bool isLesson, int startYear, int endYear, string majorId, string lecturerType)
        {
            IEnumerable query_classes = lecturerType != "-1"
                ? unitOfWork.ClassSectionRepository.GetYearStatistics(isLesson, startYear, endYear, majorId, lecturerType)
                : unitOfWork.ClassSectionRepository.GetYearStatisticsAll(isLesson, startYear, endYear, majorId);
            return Json(query_classes, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "BCN khoa, Bộ môn")]
        public ActionResult GetYearsubjects(int startYear, int endYear, string majorId, string lecturerId)
        {
            return Json(unitOfWork.ClassSectionRepository.GetYearsubjects(startYear, endYear, majorId, lecturerId), JsonRequestBehavior.AllowGet);
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
            IEnumerable<ClassSectionDTO> query_classes = unitOfWork.ClassSectionRepository.GetTimetableStatistics(termId);

            // Populate timetable
            DateTime startDate = term.start_date;
            DateTime endDate = new DateTime();
            int startWeek = term.start_week;
            int endWeek = query_classes.Any() ? query_classes.Max(c => c.EndWeek) : 1;
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
            return PartialView("_Timetable", new TimetableViewModel
            {
                ClassSectionDTOs = unitOfWork.ClassSectionRepository.GetClassInWeek(query_classes, currentWeek),
                LecturerDTOs = unitOfWork.UserRepository.GetLecturers()
            });
        }

        [HttpGet]
        public ActionResult Personal()
        {
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            ViewData["year"] = new SelectList(unitOfWork.TermRepository.GetYears(), "schoolyear", "schoolyear");
            return View();
        }

        [HttpGet]
        public ActionResult GetPersonalChart(bool isLesson, string type, string value)
        {
            ViewData["isLesson"] = isLesson;
            ViewData["type"] = type;
            ViewData["value"] = value;
            return PartialView("_PersonalChart");
        }

        [HttpGet]
        public JsonResult GetPersonalTermData(bool isLesson, int termId)
        {
            string userId = UserManager.FindByEmail(User.Identity.Name).Id;
            return Json(unitOfWork.ClassSectionRepository.GetPersonalTermStatistics(isLesson, termId, userId), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPersonalYearData(bool isLesson, int startYear, int endYear)
        {
            string userId = UserManager.FindByEmail(User.Identity.Name).Id;
            return Json(unitOfWork.ClassSectionRepository.GetPersonalYearStatistics(isLesson, startYear, endYear, userId), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Remuneration()
        {
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            return View();
        }

        [HttpGet]
        public ActionResult GetRemunerationChart(int value)
        {
            ViewData["value"] = value;
            return PartialView("_Remuneration");
        }

        [HttpGet]
        public ActionResult GetRemunerationData(int termId)
        {
            // Declare variables
            term term = unitOfWork.TermRepository.GetTermByID(termId);
            int startYear = term.start_year;
            int endYear = term.end_year;
            coefficient coefficient = unitOfWork.CoefficientRepository.GetCoefficientInYear(startYear, endYear);
            IEnumerable<LecturerRankDTO> lecturerRanks = unitOfWork.LecturerRankRepository.GetLecturerRanksInTerm(termId);
            List<RemunerationDTO> remunerationDTOs = new List<RemunerationDTO>();

            foreach (LecturerRankDTO rank in lecturerRanks)
            {
                // Reset values in each loop
                decimal teachingRemuneration = decimal.Zero;

                // Check if lecturer have been assigned a rank
                if (rank.Id != null)
                {
                    // Get classes in term of lecturer
                    IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetPersonalClassesInTerm(termId, rank.LecturerId);
                    foreach (class_section item in query_classes)
                    {
                        teachingRemuneration += item.total_lesson * RemunerationController.CalculateRemuneration(item, coefficient);
                    }

                    // Check if remuneration hours is larger than 0
                    if (teachingRemuneration > 0)
                    {
                        remunerationDTOs.Add(new RemunerationDTO
                        {
                            StaffId = rank.StaffId,
                            FullName = rank.FullName,
                            AcademicDegreeRankId = rank.AcademicDegreeRankId,
                            Remuneration = Math.Round(teachingRemuneration)
                        });
                    }
                }
            }
            return Json(remunerationDTOs.OrderByDescending(r => r.Remuneration), JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}