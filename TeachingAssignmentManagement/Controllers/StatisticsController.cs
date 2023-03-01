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
        [Authorize(Roles = "BCN khoa, Bộ môn")]
        public ActionResult Remuneration()
        {
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            ViewData["major"] = new SelectList(unitOfWork.MajorRepository.GetMajors(), "id", "name");
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "BCN khoa, Bộ môn")]
        public ActionResult GetRemunerationChart(bool isLesson, int value, string major)
        {
            ViewData["isLesson"] = isLesson;
            ViewData["value"] = value;
            ViewData["major"] = major;
            ViewData["majorAbb"] = major != "-1" ? unitOfWork.MajorRepository.GetMajorByID(major).abbreviation : "tất cả";
            return PartialView("_Remuneration");
        }

        [HttpGet]
        [Authorize(Roles = "BCN khoa, Bộ môn")]
        public ActionResult GetRemunerationData(bool isLesson, int termId, string majorId)
        {
            // Declare variables
            term term = unitOfWork.TermRepository.GetTermByID(termId);
            coefficient coefficient = unitOfWork.CoefficientRepository.GetCoefficientInYear(term.start_year, term.end_year);
            IEnumerable<lecturer> lecturers = unitOfWork.UserRepository.GetFacultyMembersInTerm(termId, majorId);

            // Check if this term and major have data
            bool haveData = unitOfWork.ClassSectionRepository.CheckClassesInTermMajor(termId, majorId);
            List<RemunerationDTO> remunerationDTOs = haveData
                ? !isLesson
                    ? GetRemunerationData(termId, majorId, coefficient, lecturers)
                    : GetRemunerationDataByLesson(termId, majorId, coefficient, lecturers)
                : new List<RemunerationDTO>();
            return Json(remunerationDTOs.OrderByDescending(r => r.RemunerationHours), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "BCN khoa, Bộ môn")]
        public ActionResult GetRemunerationSubjects(int termId, string majorId, string lecturerId)
        {
            // Get classes in term of lecturer
            term term = unitOfWork.TermRepository.GetTermByID(termId);
            coefficient coefficient = unitOfWork.CoefficientRepository.GetCoefficientInYear(term.start_year, term.end_year);
            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetPersonalClassesInTermOrderBySubject(termId, majorId, lecturerId);
            IEnumerable<subject> query_subjects = query_classes.Select(c => c.subject).Distinct();
            List<SubjectDTO> subjects = new List<SubjectDTO>();
            string previousSubjectId = string.Empty;
            foreach (var item in query_subjects)
            {
                // Loop through each subjects to find classes
                decimal remunerationHours = decimal.Zero;
                IEnumerable<class_section> query_subjectClasses = query_classes.Where(c => c.subject.subject_id == item.subject_id);
                int? subjectHours = 0;
                foreach (var subjectClass in query_subjectClasses)
                {
                    int subjectTotalLesson = subjectClass.total_lesson.GetValueOrDefault(0);
                    subjectHours += subjectTotalLesson;
                    remunerationHours += subjectTotalLesson * RemunerationController.CalculateRemuneration(subjectClass, coefficient);
                }
                subjects.Add(new SubjectDTO
                {
                    Id = item.subject_id,
                    Name = item.name,
                    Credits = item.credits,
                    Major = item.major.name,
                    Hours = subjectHours,
                    RemunerationHours = Math.Round(remunerationHours)
                });
            }
            return Json(subjects, JsonRequestBehavior.AllowGet);
        }

        private List<RemunerationDTO> GetRemunerationData(int termId, string majorId, coefficient coefficient, IEnumerable<lecturer> lecturers)
        {
            List<RemunerationDTO> remunerationDTOs = new List<RemunerationDTO>();
            foreach (lecturer lecturer in lecturers)
            {
                // Reset values in each loop
                int subjectCount = 0,
                    classCount = 0;
                int? originalHours = 0;
                decimal remunerationHours = decimal.Zero;
                string previousSubjectId = string.Empty;

                // Get classes in term of lecturer
                IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetPersonalClassesInTermOrderBySubject(termId, majorId, lecturer.id);
                foreach (class_section item in query_classes)
                {
                    int totalLesson = item.total_lesson.GetValueOrDefault(0);

                    // Count subjects and classes of lecturer
                    if (item.subject.subject_id != previousSubjectId)
                    {
                        subjectCount++;
                    }
                    classCount++;

                    // Sum up remuneration hours for this class
                    originalHours += totalLesson;
                    remunerationHours += totalLesson * RemunerationController.CalculateRemuneration(item, coefficient);
                    previousSubjectId = item.subject.subject_id;
                }

                // Check if remuneration hours is larger than 0
                if (remunerationHours > decimal.Zero)
                {
                    remunerationDTOs.Add(new RemunerationDTO
                    {
                        LecturerId = lecturer.id,
                        StaffId = lecturer.staff_id,
                        FullName = lecturer.full_name,
                        OriginalHours = originalHours,
                        SubjectCount = subjectCount,
                        ClassCount = classCount,
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
                int subjectCount = 0,
                    classCount = 0;
                int? originalHours = 0;
                decimal remunerationHours = decimal.Zero,
                        sumLesson1 = decimal.Zero,
                        sumLesson4 = decimal.Zero,
                        sumLesson7 = decimal.Zero,
                        sumLesson10 = decimal.Zero,
                        sumLesson13 = decimal.Zero;
                string previousSubjectId = string.Empty;

                // Get classes in term of lecturer
                IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetPersonalClassesInTermOrderBySubject(termId, majorId, lecturer.id);
                foreach (class_section item in query_classes)
                {
                    decimal remunerationCoefficient = RemunerationController.CalculateRemuneration(item, coefficient);
                    int startLesson = item.start_lesson_2;
                    int totalLesson = item.total_lesson.GetValueOrDefault(0);
                    decimal classRemuneration = totalLesson * remunerationCoefficient;

                    // Count subjects and classes of lecturer
                    if (item.subject.subject_id != previousSubjectId)
                    {
                        subjectCount++;
                    }
                    classCount++;

                    // Sum up remuneration hours for this class
                    originalHours += totalLesson;
                    remunerationHours += classRemuneration;
                    switch (startLesson)
                    {
                        case 1:
                            sumLesson1 += classRemuneration;
                            break;
                        case 4:
                            sumLesson4 += classRemuneration;
                            break;
                        case 7:
                            sumLesson7 += classRemuneration;
                            break;
                        case 10:
                            sumLesson10 += classRemuneration;
                            break;
                        case 13:
                            sumLesson13 += classRemuneration;
                            break;
                        default:
                            break;
                    }
                    previousSubjectId = item.subject.subject_id;
                }

                // Check if remuneration hours is larger than 0
                if (remunerationHours > decimal.Zero)
                {
                    remunerationDTOs.Add(new RemunerationDTO
                    {
                        LecturerId = lecturer.id,
                        StaffId = lecturer.staff_id,
                        FullName = lecturer.full_name,
                        SubjectCount = subjectCount,
                        ClassCount = classCount,
                        OriginalHours = originalHours,
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