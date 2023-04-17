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
using TeachingAssignmentManagement.Helpers;
using TeachingAssignmentManagement.Models;
using TeachingAssignmentManagement.Services;

namespace TeachingAssignmentManagement.Controllers
{
    [Authorize(Roles = CustomRoles.AllRoles)]
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
        [Authorize(Roles = CustomRoles.FacultyBoardOrDepartment)]
        public ActionResult Index()
        {
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            ViewData["year"] = new SelectList(unitOfWork.TermRepository.GetYears(), "schoolyear", "schoolyear");
            ViewData["major"] = new SelectList(unitOfWork.MajorRepository.GetMajors(), "id", "name");
            return View();
        }

        [HttpGet]
        [Authorize(Roles = CustomRoles.FacultyBoardOrDepartment)]
        public ActionResult GetChart(bool isLesson, string type, string value, string major, string lecturerType)
        {
            ViewData["isLesson"] = isLesson;
            ViewData["type"] = type;
            ViewData["value"] = value;
            ViewData["major"] = major;
            if (major != "-1")
            {
                major currentMajor = unitOfWork.MajorRepository.GetMajorByID(major);
                ViewData["majorAbb"] = currentMajor.abbreviation;
                ViewData["majorName"] = currentMajor.name;
            }
            else
            {
                ViewData["majorAbb"] = "TatCa";
                ViewData["majorName"] = "Tất cả";
            }
            ViewData["lecturerType"] = lecturerType;
            return PartialView("_Chart");
        }

        [HttpGet]
        [Authorize(Roles = CustomRoles.FacultyBoardOrDepartment)]
        public JsonResult GetTermData(bool isLesson, int termId, string majorId, string lecturerType)
        {
            IEnumerable query_classes = lecturerType != "-1"
                ? unitOfWork.ClassSectionRepository.GetTermStatistics(isLesson, termId, majorId, lecturerType)
                : unitOfWork.ClassSectionRepository.GetTermStatisticsAll(isLesson, termId, majorId);
            return Json(query_classes, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = CustomRoles.FacultyBoardOrDepartment)]
        public JsonResult GetTermSubjects(int termId, string majorId, string lecturerId)
        {
            return Json(unitOfWork.ClassSectionRepository.GetTermSubjects(termId, majorId, lecturerId), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = CustomRoles.FacultyBoardOrDepartment)]
        public JsonResult GetYearData(bool isLesson, int startYear, int endYear, string majorId, string lecturerType)
        {
            IEnumerable query_classes = lecturerType != "-1"
                ? unitOfWork.ClassSectionRepository.GetYearStatistics(isLesson, startYear, endYear, majorId, lecturerType)
                : unitOfWork.ClassSectionRepository.GetYearStatisticsAll(isLesson, startYear, endYear, majorId);
            return Json(query_classes, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = CustomRoles.FacultyBoardOrDepartment)]
        public JsonResult GetYearsubjects(int startYear, int endYear, string majorId, string lecturerId)
        {
            return Json(unitOfWork.ClassSectionRepository.GetYearSubjects(startYear, endYear, majorId, lecturerId), JsonRequestBehavior.AllowGet);
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
                ClassSectionDTOs = unitOfWork.ClassSectionRepository.GetTimetableInWeek(query_classes, currentWeek),
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
            return GetPersonalTermStatistics(isLesson, termId, "-1", userId);
        }

        [HttpGet]
        public JsonResult GetPersonalYearData(bool isLesson, int startYear, int endYear)
        {
            string userId = UserManager.FindByEmail(User.Identity.Name).Id;
            return GetPersonalYearStatistics(isLesson, startYear, endYear, "-1", userId);
        }

        [HttpGet]
        [Authorize(Roles = CustomRoles.FacultyBoardOrDepartment)]
        public ActionResult VisitingLecturer()
        {
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            return View();
        }

        [HttpGet]
        [Authorize(Roles = CustomRoles.FacultyBoardOrDepartment)]
        public ActionResult GetVisitingLecturerData(int[] termIds)
        {
            return PartialView("_VisitingLecturer", new VisitingLecturerStatisticsViewModel
            {
                TermIds = termIds,
                VisitingLecturerStatisticsDTOs = unitOfWork.ClassSectionRepository.GetVisitingLecturerStatistics(termIds)
            });
        }

        [HttpGet]
        [Authorize(Roles = CustomRoles.FacultyBoardOrDepartment)]
        public ActionResult Remuneration()
        {
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            ViewData["year"] = new SelectList(unitOfWork.TermRepository.GetYears(), "schoolyear", "schoolyear");
            ViewData["major"] = new SelectList(unitOfWork.MajorRepository.GetMajors(), "id", "name");
            return View();
        }

        [HttpGet]
        [Authorize(Roles = CustomRoles.FacultyBoardOrDepartment)]
        public ActionResult GetRemunerationChart(bool isLesson, string type, string value, string major)
        {
            ViewData["isLesson"] = isLesson;
            ViewData["type"] = type;
            ViewData["value"] = value;
            ViewData["major"] = major;
            if (major != "-1")
            {
                major currentMajor = unitOfWork.MajorRepository.GetMajorByID(major);
                ViewData["majorAbb"] = currentMajor.abbreviation;
                ViewData["majorName"] = currentMajor.name;
            }
            else
            {
                ViewData["majorAbb"] = "TatCa";
                ViewData["majorName"] = "Tất cả";
            }
            return PartialView("_Remuneration");
        }

        [HttpGet]
        [Authorize(Roles = CustomRoles.FacultyBoardOrDepartment)]
        public JsonResult GetTermRemunerationData(bool isLesson, int termId, string majorId)
        {
            // Declare variables
            term term = unitOfWork.TermRepository.GetTermByID(termId);
            coefficient coefficient = unitOfWork.CoefficientRepository.GetCoefficientInYear(term.start_year, term.end_year);

            // Check if coefficient is null
            if (coefficient == null)
            {
                return Json(new { error = true }, JsonRequestBehavior.AllowGet);
            }

            // Check if this term and major have data
            bool haveData = unitOfWork.ClassSectionRepository.CheckClassesInTermMajor(termId, majorId);
            IEnumerable<lecturer> lecturers = unitOfWork.UserRepository.GetFacultyMembersInTerm(termId, majorId);
            List<RemunerationDTO> remunerationDTOs = haveData
                ? !isLesson
                    ? GetTermRemunerationData(termId, majorId, coefficient, lecturers)
                    : GetTermRemunerationDataByLesson(termId, majorId, coefficient, lecturers)
                : new List<RemunerationDTO>();
            return Json(remunerationDTOs.OrderByDescending(r => r.RemunerationHours), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = CustomRoles.FacultyBoardOrDepartment)]
        public JsonResult GetYearRemunerationData(bool isLesson, int startYear, int endYear, string majorId)
        {
            // Declare variables
            coefficient coefficient = unitOfWork.CoefficientRepository.GetCoefficientInYear(startYear, endYear);

            // Check if coefficient is null
            if (coefficient == null)
            {
                return Json(new { error = true }, JsonRequestBehavior.AllowGet);
            }

            // Check if this year have data
            bool haveData = unitOfWork.ClassSectionRepository.CheckClassesInYearMajor(startYear, endYear, majorId);
            IEnumerable<lecturer> lecturers = unitOfWork.UserRepository.GetFacultyMembersInYear(startYear, endYear, majorId);
            List<RemunerationDTO> remunerationDTOs = haveData
                ? !isLesson
                    ? GetYearRemunerationData(startYear, endYear, majorId, coefficient, lecturers)
                    : GetYearRemunerationDataByLesson(startYear, endYear, majorId, coefficient, lecturers)
                : new List<RemunerationDTO>();
            return Json(remunerationDTOs.OrderByDescending(r => r.RemunerationHours), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = CustomRoles.FacultyBoardOrDepartment)]
        public JsonResult GetTermRemunerationSubjects(int termId, string majorId, string lecturerId)
        {
            return GetPersonalTermStatistics(false, termId, majorId, lecturerId);
        }

        [HttpGet]
        [Authorize(Roles = CustomRoles.FacultyBoardOrDepartment)]
        public JsonResult GetYearRemunerationSubjects(int startYear, int endYear, string majorId, string lecturerId)
        {
            return GetPersonalYearStatistics(false, startYear, endYear, majorId, lecturerId);
        }

        private JsonResult GetPersonalTermStatistics(bool isLesson, int termId, string majorId, string lecturerId)
        {
            // Get classes in term of lecturer
            term term = unitOfWork.TermRepository.GetTermByID(termId);
            coefficient coefficient = unitOfWork.CoefficientRepository.GetCoefficientInYear(term.start_year, term.end_year);

            // Check if coefficient is null
            if (coefficient == null)
            {
                return Json(new { error = true }, JsonRequestBehavior.AllowGet);
            }

            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetPersonalClassesInTermOrderBySubject(termId, majorId, lecturerId);
            IEnumerable<subject> query_subjects = query_classes.Select(c => c.subject).Distinct();
            List<SubjectDTO> subjects = new List<SubjectDTO>();
            string previousSubjectId = string.Empty;
            foreach (subject item in query_subjects)
            {
                // Loop through each subjects to find classes
                decimal remunerationHours = decimal.Zero;
                IEnumerable<class_section> query_subjectClasses = query_classes.Where(c => c.subject.id == item.id);
                int? subjectHours = 0;
                foreach (class_section subjectClass in query_subjectClasses)
                {
                    int subjectTotalLesson = subjectClass.total_lesson.GetValueOrDefault(0);
                    subjectHours += subjectTotalLesson;
                    remunerationHours += subjectTotalLesson * RemunerationService.CalculateRemuneration(subjectClass, coefficient);
                }
                if (!isLesson)
                {
                    subjects.Add(new SubjectDTO
                    {
                        Id = item.subject_id,
                        Name = item.name,
                        Credits = item.credits,
                        Major = item.major.name,
                        Hours = subjectHours,
                        RemunerationHours = Math.Round(remunerationHours),
                        TheoryCount = query_subjectClasses.Count(c => c.type == MyConstants.TheoreticalClassType),
                        PracticeCount = query_subjectClasses.Count(c => c.type == MyConstants.PracticeClassType)
                    });
                }
                else
                {
                    subjects.Add(new SubjectDTO
                    {
                        Id = item.subject_id,
                        Name = item.name,
                        Credits = item.credits,
                        Major = item.major.name,
                        Hours = subjectHours,
                        RemunerationHours = Math.Round(remunerationHours),
                        TheoryCount = query_subjectClasses.Count(c => c.type == MyConstants.TheoreticalClassType),
                        PracticeCount = query_subjectClasses.Count(c => c.type == MyConstants.PracticeClassType),
                        SumLesson1 = query_subjectClasses.Where(c => c.start_lesson_2 == 1).Sum(c => c.total_lesson),
                        SumLesson4 = query_subjectClasses.Where(c => c.start_lesson_2 == 4).Sum(c => c.total_lesson),
                        SumLesson7 = query_subjectClasses.Where(c => c.start_lesson_2 == 7).Sum(c => c.total_lesson),
                        SumLesson10 = query_subjectClasses.Where(c => c.start_lesson_2 == 10).Sum(c => c.total_lesson),
                        SumLesson13 = query_subjectClasses.Where(c => c.start_lesson_2 == 13).Sum(c => c.total_lesson)
                    });
                }
            }
            return Json(subjects.OrderByDescending(s => s.Hours), JsonRequestBehavior.AllowGet);
        }

        private JsonResult GetPersonalYearStatistics(bool isLesson, int startYear, int endYear, string majorId, string lecturerId)
        {
            // Get classes in year of lecturer
            coefficient coefficient = unitOfWork.CoefficientRepository.GetCoefficientInYear(startYear, endYear);

            // Check if coefficient is null
            if (coefficient == null)
            {
                return Json(new { error = true }, JsonRequestBehavior.AllowGet);
            }

            IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetPersonalClassesInYearOrderBySubject(startYear, endYear, majorId, lecturerId);
            IEnumerable<subject> query_subjects = query_classes.Select(c => c.subject).Distinct();
            List<SubjectDTO> subjects = new List<SubjectDTO>();
            string previousSubjectId = string.Empty;
            foreach (subject item in query_subjects)
            {
                // Loop through each subjects to find classes
                decimal remunerationHours = decimal.Zero;
                IEnumerable<class_section> query_subjectClasses = query_classes.Where(c => c.subject.id == item.id);
                int? subjectHours = 0;
                foreach (class_section subjectClass in query_subjectClasses)
                {
                    int subjectTotalLesson = subjectClass.total_lesson.GetValueOrDefault(0);
                    subjectHours += subjectTotalLesson;
                    remunerationHours += subjectTotalLesson * RemunerationService.CalculateRemuneration(subjectClass, coefficient);
                }
                if (!isLesson)
                {
                    subjects.Add(new SubjectDTO
                    {
                        Id = item.subject_id,
                        Name = item.name,
                        Credits = item.credits,
                        Major = item.major.name,
                        Hours = subjectHours,
                        RemunerationHours = Math.Round(remunerationHours),
                        TheoryCount = query_subjectClasses.Count(c => c.type == MyConstants.TheoreticalClassType),
                        PracticeCount = query_subjectClasses.Count(c => c.type == MyConstants.PracticeClassType)
                    });
                }
                else
                {
                    subjects.Add(new SubjectDTO
                    {
                        Id = item.subject_id,
                        Name = item.name,
                        Credits = item.credits,
                        Major = item.major.name,
                        Hours = subjectHours,
                        RemunerationHours = Math.Round(remunerationHours),
                        TheoryCount = query_subjectClasses.Count(c => c.type == MyConstants.TheoreticalClassType),
                        PracticeCount = query_subjectClasses.Count(c => c.type == MyConstants.PracticeClassType),
                        SumLesson1 = query_subjectClasses.Where(c => c.start_lesson_2 == 1).Sum(c => c.total_lesson),
                        SumLesson4 = query_subjectClasses.Where(c => c.start_lesson_2 == 4).Sum(c => c.total_lesson),
                        SumLesson7 = query_subjectClasses.Where(c => c.start_lesson_2 == 7).Sum(c => c.total_lesson),
                        SumLesson10 = query_subjectClasses.Where(c => c.start_lesson_2 == 10).Sum(c => c.total_lesson),
                        SumLesson13 = query_subjectClasses.Where(c => c.start_lesson_2 == 13).Sum(c => c.total_lesson)
                    });
                }
            }
            return Json(subjects.OrderByDescending(s => s.Hours), JsonRequestBehavior.AllowGet);
        }

        private List<RemunerationDTO> GetTermRemunerationData(int termId, string majorId, coefficient coefficient, IEnumerable<lecturer> lecturers)
        {
            List<RemunerationDTO> remunerationDTOs = new List<RemunerationDTO>();
            foreach (lecturer lecturer in lecturers)
            {
                // Reset values in each loop
                int subjectCount = 0,
                    classCount = 0;
                int? originalHours = 0;
                decimal remunerationHours = decimal.Zero;
                List<string> classesTaught = new List<string>();
                string previousSubjectId = string.Empty;

                // Get classes in term of lecturer
                IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetPersonalClassesInTermOrderBySubject(termId, majorId, lecturer.id);
                foreach (class_section item in query_classes)
                {
                    int totalLesson = item.total_lesson.GetValueOrDefault(0);
                    decimal classRemuneration = totalLesson * RemunerationService.CalculateRemuneration(item, coefficient);

                    // Count subjects and classes of lecturer
                    if (item.subject_id != previousSubjectId)
                    {
                        subjectCount++;
                    }
                    classCount++;

                    // Sum up remuneration hours for this class
                    originalHours += totalLesson;
                    remunerationHours += classRemuneration;
                    classesTaught.Add(item.class_section_id + "-" + item.subject.name + " (" + Math.Round(classRemuneration) + " tiết)");
                    previousSubjectId = item.subject_id;
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
                        ClassesTaught = classesTaught,
                        RemunerationHours = Math.Round(remunerationHours)
                    });
                }
            }
            return remunerationDTOs;
        }

        private List<RemunerationDTO> GetTermRemunerationDataByLesson(int termId, string majorId, coefficient coefficient, IEnumerable<lecturer> lecturers)
        {
            List<RemunerationDTO> remunerationDTOs = new List<RemunerationDTO>();
            foreach (lecturer lecturer in lecturers)
            {
                // Reset values in each loop
                int subjectCount = 0,
                    classCount = 0;
                int? originalHours = 0,
                     originalSumLesson1 = 0,
                     originalSumLesson4 = 0,
                     originalSumLesson7 = 0,
                     originalSumLesson10 = 0,
                     originalSumLesson13 = 0;
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
                    int startLesson = item.start_lesson_2;
                    int totalLesson = item.total_lesson.GetValueOrDefault(0);
                    decimal classRemuneration = totalLesson * RemunerationService.CalculateRemuneration(item, coefficient);

                    // Count subjects and classes of lecturer
                    if (item.subject_id != previousSubjectId)
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
                            originalSumLesson1 += totalLesson;
                            sumLesson1 += classRemuneration;
                            break;
                        case 4:
                            originalSumLesson4 += totalLesson;
                            sumLesson4 += classRemuneration;
                            break;
                        case 7:
                            originalSumLesson7 += totalLesson;
                            sumLesson7 += classRemuneration;
                            break;
                        case 10:
                            originalSumLesson10 += totalLesson;
                            sumLesson10 += classRemuneration;
                            break;
                        case 13:
                            originalSumLesson13 += totalLesson;
                            sumLesson13 += classRemuneration;
                            break;
                        default:
                            break;
                    }
                    previousSubjectId = item.subject_id;
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
                        OriginalSumLesson1 = originalSumLesson1,
                        OriginalSumLesson4 = originalSumLesson4,
                        OriginalSumLesson7 = originalSumLesson7,
                        OriginalSumLesson10 = originalSumLesson10,
                        OriginalSumLesson13 = originalSumLesson13,
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

        private List<RemunerationDTO> GetYearRemunerationData(int startYear, int endYear, string majorId, coefficient coefficient, IEnumerable<lecturer> lecturers)
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

                // Get classes in year of lecturer
                IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetPersonalClassesInYearOrderBySubject(startYear, endYear, majorId, lecturer.id);
                foreach (class_section item in query_classes)
                {
                    int totalLesson = item.total_lesson.GetValueOrDefault(0);

                    // Count subjects and classes of lecturer
                    if (item.subject_id != previousSubjectId)
                    {
                        subjectCount++;
                    }
                    classCount++;

                    // Sum up remuneration hours for this class
                    originalHours += totalLesson;
                    remunerationHours += totalLesson * RemunerationService.CalculateRemuneration(item, coefficient);
                    previousSubjectId = item.subject_id;
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

        private List<RemunerationDTO> GetYearRemunerationDataByLesson(int startYear, int endYear, string majorId, coefficient coefficient, IEnumerable<lecturer> lecturers)
        {
            List<RemunerationDTO> remunerationDTOs = new List<RemunerationDTO>();
            foreach (lecturer lecturer in lecturers)
            {
                // Reset values in each loop
                int subjectCount = 0,
                    classCount = 0;
                int? originalHours = 0,
                     originalSumLesson1 = 0,
                     originalSumLesson4 = 0,
                     originalSumLesson7 = 0,
                     originalSumLesson10 = 0,
                     originalSumLesson13 = 0;
                decimal remunerationHours = decimal.Zero,
                        sumLesson1 = decimal.Zero,
                        sumLesson4 = decimal.Zero,
                        sumLesson7 = decimal.Zero,
                        sumLesson10 = decimal.Zero,
                        sumLesson13 = decimal.Zero;
                string previousSubjectId = string.Empty;

                // Get classes in year of lecturer
                IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetPersonalClassesInYearOrderBySubject(startYear, endYear, majorId, lecturer.id);
                foreach (class_section item in query_classes)
                {
                    decimal remunerationCoefficient = RemunerationService.CalculateRemuneration(item, coefficient);
                    int startLesson = item.start_lesson_2;
                    int totalLesson = item.total_lesson.GetValueOrDefault(0);
                    decimal classRemuneration = totalLesson * remunerationCoefficient;

                    // Count subjects and classes of lecturer
                    if (item.subject_id != previousSubjectId)
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
                            originalSumLesson1 += totalLesson;
                            sumLesson1 += classRemuneration;
                            break;
                        case 4:
                            originalSumLesson4 += totalLesson;
                            sumLesson4 += classRemuneration;
                            break;
                        case 7:
                            originalSumLesson7 += totalLesson;
                            sumLesson7 += classRemuneration;
                            break;
                        case 10:
                            originalSumLesson10 += totalLesson;
                            sumLesson10 += classRemuneration;
                            break;
                        case 13:
                            originalSumLesson13 += totalLesson;
                            sumLesson13 += classRemuneration;
                            break;
                        default:
                            break;
                    }
                    previousSubjectId = item.subject_id;
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
                        OriginalSumLesson1 = originalSumLesson1,
                        OriginalSumLesson4 = originalSumLesson4,
                        OriginalSumLesson7 = originalSumLesson7,
                        OriginalSumLesson10 = originalSumLesson10,
                        OriginalSumLesson13 = originalSumLesson13,
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