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
            ViewData["major"] = new SelectList(unitOfWork.MajorRepository.GetMajors(), "id", "name");
            return View();
        }

        [HttpGet]
        public ActionResult GetRemunerationChart(bool isLesson, int value, string major)
        {
            ViewData["isLesson"] = isLesson;
            ViewData["value"] = value;
            ViewData["major"] = major;
            ViewData["majorAbb"] = major != "-1" ? unitOfWork.MajorRepository.GetMajorByID(major).abbreviation : "tất cả";
            return PartialView("_Remuneration");
        }

        [HttpGet]
        public ActionResult GetRemunerationData(bool isLesson, int termId, string majorId)
        {
            // Declare variables
            term term = unitOfWork.TermRepository.GetTermByID(termId);
            int startYear = term.start_year;
            int endYear = term.end_year;
            coefficient coefficient = unitOfWork.CoefficientRepository.GetCoefficientInYear(startYear, endYear);
            IEnumerable<lecturer> lecturers = unitOfWork.UserRepository.GetFacultyMembersInTerm(termId, majorId).ToList();

            // Check if this term and major have data
            bool haveData = unitOfWork.ClassSectionRepository.CheckClassesInTermMajor(termId, majorId);
            List<RemunerationDTO> remunerationDTOs = haveData
                ? !isLesson
                    ? GetRemunerationData(termId, majorId, coefficient, lecturers)
                    : GetRemunerationDataByLesson(termId, majorId, coefficient, lecturers)
                : new List<RemunerationDTO>();
            return Json(remunerationDTOs.OrderByDescending(r => r.RemunerationHours), JsonRequestBehavior.AllowGet);
        }

        private List<RemunerationDTO> GetRemunerationData(int termId, string majorId, coefficient coefficient, IEnumerable<lecturer> lecturers)
        {
            List<RemunerationDTO> remunerationDTOs = new List<RemunerationDTO>();
            foreach (lecturer lecturer in lecturers)
            {
                // Reset values in each loop
                decimal remunerationHours = decimal.Zero;

                // Get classes in term of lecturer
                IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetPersonalClassesInTerm(termId, majorId, lecturer.id);
                foreach (class_section item in query_classes)
                {
                    remunerationHours += item.total_lesson.GetValueOrDefault(0) * RemunerationController.CalculateRemuneration(item, coefficient);
                }

                // Check if remuneration hours is larger than 0
                if (remunerationHours > decimal.Zero)
                {
                    remunerationDTOs.Add(new RemunerationDTO
                    {
                        StaffId = lecturer.staff_id,
                        FullName = lecturer.full_name,
                        AcademicDegreeRankId = "hehe",
                        RemunerationHours = Math.Round(remunerationHours)
                    });
                }
            }
            return remunerationDTOs;
        }

        private List<RemunerationDTO> GetRemunerationDataByLesson(int termId, string majorId, coefficient coefficient, IEnumerable<lecturer> lecturers)
        {
            List<RemunerationDTO> remunerationDTOs = new List<RemunerationDTO>();
            foreach (lecturer lecturer in lecturers)
            {
                // Reset values in each loop
                decimal remunerationHours = decimal.Zero,
                        sumLesson1 = decimal.Zero,
                        sumLesson4 = decimal.Zero,
                        sumLesson7 = decimal.Zero,
                        sumLesson10 = decimal.Zero,
                        sumLesson13 = decimal.Zero;

                // Get classes in term of lecturer
                IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetPersonalClassesInTerm(termId, majorId, lecturer.id);
                foreach (class_section item in query_classes)
                {
                    decimal remunerationCoefficient = RemunerationController.CalculateRemuneration(item, coefficient);
                    int totalLesson = item.total_lesson.GetValueOrDefault(0);
                    decimal classRemuneration = totalLesson * remunerationCoefficient;
                    remunerationHours += classRemuneration;
                    sumLesson1 += item.start_lesson_2 == 1 ? classRemuneration : decimal.Zero;
                    sumLesson4 += item.start_lesson_2 == 4 ? classRemuneration : decimal.Zero;
                    sumLesson7 += item.start_lesson_2 == 7 ? classRemuneration : decimal.Zero;
                    sumLesson10 += item.start_lesson_2 == 10 ? classRemuneration : decimal.Zero;
                    sumLesson13 += item.start_lesson_2 == 13 ? classRemuneration : decimal.Zero;
                }

                // Check if remuneration hours is larger than 0
                if (remunerationHours > decimal.Zero)
                {
                    remunerationDTOs.Add(new RemunerationDTO
                    {
                        StaffId = lecturer.staff_id,
                        FullName = lecturer.full_name,
                        AcademicDegreeRankId = "hehe",
                        RemunerationHours = Math.Round(remunerationHours),
                        SumLesson1 = Math.Round(sumLesson1),
                        SumLesson4 = Math.Round(sumLesson4),
                        SumLesson7 = Math.Round(sumLesson7),
                        SumLesson10 = Math.Round(sumLesson10),
                        SumLesson13 = Math.Round(sumLesson13)
                    });
                }
            }
            return remunerationDTOs;
        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}