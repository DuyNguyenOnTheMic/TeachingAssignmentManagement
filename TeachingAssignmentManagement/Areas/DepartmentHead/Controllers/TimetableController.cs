using ClosedXML.Excel;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Hubs;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Areas.DepartmentHead.Controllers
{
    [Authorize(Roles = "Bộ môn")]
    public class TimetableController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly UnitOfWork unitOfWork = new UnitOfWork();

        public TimetableController()
        {
        }

        public TimetableController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
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

        // GET: DepartmentHead/Timetable
        public ActionResult Index()
        {
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            return View();
        }

        [HttpGet]
        public ActionResult GetPersonalData(int termId, int week)
        {
            // Declare variables
            string userId = "32e17ae6-c72f-4f20-b865-251e9ee8e71d";
            term term = unitOfWork.TermRepository.GetTermByID(termId);
            IEnumerable<CurriculumClassDTO> query_classes = unitOfWork.CurriculumClassRepository.GetTimetable(termId, userId);
            if (!query_classes.Any())
            {
                // Return not found error message
                return Json(new { error = true, message = "Học kỳ này chưa có dữ liệu phân công của bạn!" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                // Populate personal timetable
                DateTime startDate = term.start_date;
                DateTime endDate = new DateTime();
                int startWeek = term.start_week;
                int endWeek = query_classes.Max(c => c.EndWeek);
                int currentWeek = 0;
                string weekLabel = string.Empty;

                if (week > 0)
                {
                    // Set week based on user's selection
                    startDate = startDate.AddDays((week - 1) * 7).Date;
                    endDate = startDate.AddDays(6).Date;
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
                            if (date.Date == DateTime.Today)
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
                        // Set current week in case no current week found
                        currentWeek = endWeek;
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
                return PartialView("_PersonalTimetable", new TimetableViewModels
                {
                    CurriculumClassDTOs = unitOfWork.CurriculumClassRepository.GetClassInWeek(query_classes, currentWeek).ToList()
                });
            }
        }

        [HttpGet]
        public ActionResult GetData(int termId, string majorId)
        {
            IEnumerable<CurriculumClassDTO> query_classes = unitOfWork.CurriculumClassRepository.GetAssignTimetable(termId, majorId);
            ViewBag.curriculums = unitOfWork.CurriculumRepository.GetCurriculums(query_classes);
            ViewBag.lecturers = new SelectList(unitOfWork.UserRepository.GetLecturers(), "id", "full_name");
            return PartialView("_Timetable", new TimetableViewModels
            {
                CurriculumClassDTOs = query_classes.ToList()
            });
        }

        [HttpGet]
        public FileResult Export(int termId, string majorId)
        {
            DataTable dt = new DataTable("PHÂN CÔNG HK" + termId);
            dt.Columns.AddRange(new DataColumn[30]
            {
                new DataColumn("MaGocLHP"), new DataColumn("Mã MH"), new DataColumn("Mã LHP"), new DataColumn("Tên HP"),
                new DataColumn("Số TC"), new DataColumn("Loại HP"), new DataColumn("Mã Lớp"), new DataColumn("TSMH"),
                new DataColumn("Số Tiết Đã xếp"), new DataColumn("PH"), new DataColumn("Thứ"), new DataColumn("Tiết BĐ"),
                new DataColumn("Số Tiết"), new DataColumn("Tiết Học"), new DataColumn("Phòng"), new DataColumn("Mã CBGD"),
                new DataColumn("Tên CBGD"), new DataColumn("PH_X"), new DataColumn("Sức Chứa"), new DataColumn("SiSoTKB"),
                new DataColumn("Trống"), new DataColumn("Tình Trạng LHP"), new DataColumn("TuanHoc2"), new DataColumn("ThuS"),
                new DataColumn("TietS"), new DataColumn("Số SVĐK"), new DataColumn("Tuần BD"), new DataColumn("Tuần KT"),
                new DataColumn("Ghi Chú 1"), new DataColumn("Ghi chú 2")
            });

            IEnumerable<curriculum_class> classes = unitOfWork.CurriculumClassRepository.GetExportData(termId, majorId);
            foreach (curriculum_class item in classes)
            {
                // Add data to table
                dt.Rows.Add(item.original_id, item.curriculum_id, item.curriculum_class_id, item.curriculum.name,
                    item.curriculum.credits, item.type, item.student_class_id, item.minimum_student, item.total_lesson,
                    item.room.room_2, item.day, item.start_lesson, item.lesson_number, item.lesson_time, item.room_id,
                    item.lecturer?.staff_id, item.lecturer?.full_name, item.room.type, item.room.capacity, item.student_number,
                    item.free_slot, item.state, item.learn_week, item.day_2, item.start_lesson_2,
                    item.student_registered_number, item.start_week, item.end_week, item.note_1, item.note_2);
            }

            using (XLWorkbook workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add(dt);
                for (int row = 1; row <= dt.Rows.Count; row++)
                {
                    // Color lecturer id and Lecturer name if null
                    IXLCell lecturerId = worksheet.Cell(row, 16);
                    IXLCell lecturerName = worksheet.Cell(row, 17);
                    if (string.IsNullOrEmpty(lecturerId.Value.ToString()))
                    {
                        lecturerId.Style.Fill.BackgroundColor = XLColor.Yellow;
                        lecturerName.Style.Fill.BackgroundColor = XLColor.Yellow;
                    }
                }
                IXLTable table = worksheet.Table(0);
                table.HeadersRow().Style.Font.Bold = true;
                table.Theme = XLTableTheme.None;
                using (MemoryStream stream = new MemoryStream())
                {
                    // Export to Excel file
                    string fileName = "CNTT ThoiKhoaBieu_HK" + termId + "_Nganh" + majorId + ".xlsx";
                    workbook.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        [HttpGet]
        public ActionResult Assign()
        {
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            ViewData["major"] = new SelectList(unitOfWork.MajorRepository.GetMajors(), "id", "name");
            return View();
        }

        [HttpPost]
        public JsonResult Assign(int id, string lecturerId)
        {
            // Declare variables
            curriculum_class curriculumClass = unitOfWork.CurriculumClassRepository.GetClassByID(id);
            lecturerId = ToNullableString(lecturerId);

            // Update lecturer id of curriculum class
            curriculumClass.lecturer_id = lecturerId;
            unitOfWork.Save();

            // Send signal to SignalR Hub
            CurriculumClassHub.BroadcastData(id, lecturerId, curriculumClass.lecturer?.full_name, true);
            return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult CheckState(int id, int termId, string lecturerId)
        {
            if (lecturerId != string.Empty)
            {
                // Declare variables
                curriculum_class curriculumClass = unitOfWork.CurriculumClassRepository.GetClassByID(id);
                term term = unitOfWork.TermRepository.GetTermByID(termId);
                IEnumerable<curriculum_class> query_classWeek = unitOfWork.CurriculumClassRepository.GetClassesInTerm(termId, lecturerId);
                IEnumerable<curriculum_class> query_classDay = unitOfWork.CurriculumClassRepository.GetClassesInDay(query_classWeek, curriculumClass.day_2);
                IEnumerable<curriculum_class> query_classLesson = unitOfWork.CurriculumClassRepository.GetClassesInLesson(query_classDay, curriculumClass.start_lesson_2);

                // Check not duplicate class in the same time
                if (query_classLesson.Count() >= 1)
                {
                    IEnumerable classes = query_classLesson.Select(c => new
                    {
                        classId = c.curriculum_class_id,
                        curriculumName = c.curriculum.name,
                        classDay = c.day,
                        lessonTime = c.lesson_time,
                        majorName = c.major.name
                    }).ToList();
                    return Json(new { duplicate = true, message = "Giảng viên này đã có lớp trong tiết học này!", classList = classes }, JsonRequestBehavior.AllowGet);
                }

                // Check maximum lessons in a day
                if (query_classDay.Sum(c => c.lesson_number) + curriculumClass.lesson_number > term.max_lesson)
                {
                    IEnumerable classes = query_classDay.Select(c => new
                    {
                        classId = c.curriculum_class_id,
                        curriculumName = c.curriculum.name,
                        classDay = c.day,
                        lessonTime = c.lesson_time,
                        majorName = c.major.name
                    }).ToList();
                    return Json(new { maxLesson = true, message = "Giảng viên này đã đạt số tiết tối đa trong 1 ngày!", classList = classes }, JsonRequestBehavior.AllowGet);
                }

                // Check maximum classes in a week
                if (query_classWeek.Count() >= term.max_class)
                {
                    IEnumerable classes = query_classWeek.Select(c => new
                    {
                        classId = c.curriculum_class_id,
                        curriculumName = c.curriculum.name,
                        classDay = c.day,
                        lessonTime = c.lesson_time,
                        majorName = c.major.name
                    }).ToList();
                    return Json(new { maxCLass = true, message = "Giảng viên này đạt số lớp tối đa trong 1 tuần!", classList = classes }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
            }

            unitOfWork.Dispose();
            base.Dispose(disposing);
        }

        #region Helpers
        public static string ToNullableString(string value)
        {
            // Check if string is empty
            return value != null && string.IsNullOrEmpty(value.Trim()) ? null : value;
        }
        #endregion
    }
}