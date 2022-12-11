using ClosedXML.Excel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Hubs;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Controllers
{
    [Authorize(Roles = "BCN khoa, Bộ môn, Giảng viên")]
    public class TimetableController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly UnitOfWork unitOfWork;

        public TimetableController()
        {
            unitOfWork = new UnitOfWork(new CP25Team03Entities());
        }

        public TimetableController(UnitOfWork unitOfWork)
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
        public ActionResult Index()
        {
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            return View();
        }

        [HttpGet]
        public ActionResult GetPersonalData(int termId, int week)
        {
            // Declare variables
            string userId = UserManager.FindByEmail(User.Identity.Name).Id;
            term term = unitOfWork.TermRepository.GetTermByID(termId);
            IEnumerable<CurriculumClassDTO> query_classes = unitOfWork.CurriculumClassRepository.GetTimetable(termId, userId);
            if (!query_classes.Any())
            {
                // Return not found error message
                return Json(new { error = true, message = "Học kỳ này chưa có dữ liệu phân công của bạn" }, JsonRequestBehavior.AllowGet);
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
                return PartialView("_PersonalTimetable", new TimetableViewModels
                {
                    CurriculumClassDTOs = unitOfWork.CurriculumClassRepository.GetClassInWeek(query_classes, currentWeek).ToList()
                });
            }
        }

        [HttpGet]
        public ActionResult GetData(int termId, string majorId)
        {
            IEnumerable<CurriculumClassDTO> query_classes = majorId != "-1"
                ? unitOfWork.CurriculumClassRepository.GetAssignTimetable(termId, majorId)
                : unitOfWork.CurriculumClassRepository.GetTermAssignTimetable(termId);
            ViewBag.curriculums = unitOfWork.CurriculumRepository.GetCurriculums(query_classes);
            ViewBag.lecturers = new SelectList(unitOfWork.UserRepository.GetLecturers(), "id", "full_name");
            return PartialView("_Timetable", new TimetableViewModels
            {
                CurriculumClassDTOs = query_classes.ToList()
            });
        }

        [HttpGet]
        [Authorize(Roles = "BCN khoa")]
        public ActionResult Import()
        {
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            ViewData["major"] = new SelectList(unitOfWork.MajorRepository.GetMajors(), "id", "name");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "BCN khoa")]
        public ActionResult Import(HttpPostedFileBase postedFile, int term, string major, bool isUpdate)
        {
            string path = Server.MapPath("~/Uploads/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string filePath = path + Path.GetFileName(postedFile.FileName);
            postedFile.SaveAs(filePath);

            string conString = ConfigurationManager.ConnectionStrings["ExcelConString"].ConnectionString;

            DataTable dt = new DataTable();
            conString = string.Format(conString, filePath);

            using (OleDbConnection connExcel = new OleDbConnection(conString))
            {
                using (OleDbCommand cmdExcel = new OleDbCommand())
                {
                    using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                    {
                        cmdExcel.Connection = connExcel;

                        //Get the name of First Sheet.
                        connExcel.Open();
                        DataTable dtExcelSchema;
                        dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                        connExcel.Close();

                        //Read Data from First Sheet.
                        connExcel.Open();
                        cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                        odaExcel.SelectCommand = cmdExcel;
                        odaExcel.Fill(dt);
                        connExcel.Close();
                    }
                }
            }

            string userId = UserManager.FindByEmail(User.Identity.Name).Id;

            // Check if lecturer has filled in personal information
            lecturer lecturer = unitOfWork.UserRepository.GetLecturerByID(userId);
            if (lecturer?.staff_id == null || lecturer?.full_name == null)
            {
                Response.Write($"Bạn chưa điền đầy đủ thông tin của bạn (<strong>mã giảng viên</strong> và <strong>tên giảng viên</strong>) để hệ thống có thể ghi nhận phân công bởi bạn.");
                return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
            }

            if (!isUpdate)
            {
                // Check if this term and major already has data
                curriculum_class query_term_major = unitOfWork.CurriculumClassRepository.CheckTermMajor(term, major);
                if (query_term_major != null)
                {
                    Response.Write($"Học kỳ và ngành này đã có dữ liệu trong hệ thống, bạn muốn cập nhật hay thay thế thời khoá biểu?");
                    return new HttpStatusCodeResult(HttpStatusCode.Conflict);
                }
            }

            // Trim column name string
            foreach (DataColumn col in dt.Columns)
            {
                col.ColumnName = col.ColumnName.Trim();
            }

            // Validate all columns
            string isValid = ValidateColumns(dt);
            if (isValid != null)
            {
                Response.Write($"Có vẻ như bạn đã sai hoặc thiếu tên cột <strong>" + isValid + "</strong>, vui lòng kiểm tra lại tệp tin!");
                return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
            }

            int itemsCount = dt.Rows.Count;
            TimetableViewModels timetableViewModels = new TimetableViewModels();
            List<curriculum_class> curriculumClassList = new List<curriculum_class>();
            IEnumerable<curriculum_class> query_curriculumClassWhere = curriculumClassList;
            if (isUpdate)
            {
                // Query Curriculum classes of this term and major
                query_curriculumClassWhere = unitOfWork.CurriculumClassRepository.GetClassesByTerm(term);
            }

            // Create a list for storing error Lecturers
            List<Tuple<string, string>> errorLecturerList = new List<Tuple<string, string>>();
            List<Tuple<string, string, string, string, string, string>> errorAssignList = new List<Tuple<string, string, string, string, string, string>>();

            try
            {
                //Insert records to database table.
                foreach (DataRow row in dt.Rows)
                {
                    // Declare all columns
                    string originalId = row["MaGocLHP"].ToString();
                    string curriculumId = row["Mã MH"].ToString();
                    string curriculumClassid = row["Mã LHP"].ToString();
                    string name = row["Tên HP"].ToString();
                    string credits = row["Số TC"].ToString();
                    string type = row["Loại HP"].ToString();
                    string studentClassId = row["Mã Lớp"].ToString();
                    string minimumStudent = row["TSMH"].ToString();
                    string totalLesson = row["Số Tiết Đã xếp"].ToString();
                    string room2 = row["PH"].ToString();
                    string day = row["Thứ"].ToString();
                    string startLesson = row["Tiết BĐ"].ToString();
                    string lessonNumber = row["Số Tiết"].ToString();
                    string lessonTime = row["Tiết Học"].ToString();
                    string roomId = row["Phòng"].ToString();
                    string lecturerId = row["Mã CBGD"].ToString();
                    string fullName = row["Tên CBGD"].ToString();
                    string roomType = row["PH_X"].ToString();
                    string capacity = row["Sức Chứa"].ToString();
                    string studentNumber = row["SiSoTKB"].ToString();
                    string freeSlot = row["Trống"].ToString();
                    string state = row["Tình Trạng LHP"].ToString();
                    string learnWeek = row["TuanHoc2"].ToString();
                    string day2 = row["ThuS"].ToString();
                    string startLesson2 = row["TietS"].ToString();
                    string studentRegisteredNumber = row["Số SVĐK"].ToString();
                    string startWeek = row["Tuần BD"].ToString();
                    string endWeek = row["Tuần KT"].ToString();
                    string note1 = row["Ghi Chú 1"].ToString();
                    string note2 = row["Ghi chú 2"].ToString();

                    // Check if values is null
                    string[] validRows = { originalId, curriculumId, curriculumClassid, name, credits, type, day, startLesson, lessonNumber, roomId, day2, startLesson2, startWeek, endWeek };
                    string checkNull = ValidateNotNull(validRows);
                    if (checkNull != null)
                    {
                        int excelRow = dt.Rows.IndexOf(row) + 2;
                        Response.Write($"Oops, có lỗi đã xảy ra ở dòng số <strong>" + excelRow + "</strong>, vui lòng kiểm tra lại tệp tin.");
                        return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
                    }

                    // Check if start lessons is true
                    if (!timetableViewModels.startLessons.Contains(ToInt(startLesson2)))
                    {
                        int excelRow = dt.Rows.IndexOf(row) + 2;
                        Response.Write($"Oops, có lỗi đã xảy ra ở dòng số <strong>" + excelRow + "</strong>, tiết bắt đầu phải là 1, 4, 7, 10 hoặc 13.");
                        return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
                    }

                    curriculum query_curriculum = unitOfWork.CurriculumRepository.GetCurriculumByID(curriculumId);
                    if (query_curriculum == null)
                    {
                        // Create new curriculum
                        curriculum curriculum = new curriculum()
                        {
                            id = ToNullableString(curriculumId),
                            name = ToNullableString(name),
                            credits = ToInt(credits)
                        };
                        unitOfWork.CurriculumRepository.InsertCurriculum(curriculum);
                        unitOfWork.Save();
                    }

                    room query_room = unitOfWork.RoomRepository.GetRoomByID(roomId);
                    if (query_room == null)
                    {
                        // Create new room
                        room room = new room()
                        {
                            // Add record to error list
                            id = ToNullableString(roomId),
                            room_2 = ToNullableString(room2),
                            type = ToNullableString(roomType),
                            capacity = ToNullableInt(capacity)
                        };
                        unitOfWork.RoomRepository.InsertRoom(room);
                        unitOfWork.Save();
                    }

                    string lastAssignedBy = null;
                    lecturer query_lecturer = unitOfWork.UserRepository.GetLecturerByStaffId(lecturerId);
                    if (query_lecturer == null && ToNullableString(lecturerId) != null && ToNullableString(fullName) != null)
                    {
                        // Add record to error list
                        errorLecturerList.Add(Tuple.Create(ToNullableString(lecturerId), ToNullableString(fullName)));
                    }
                    else
                    {
                        // Set last assigned by value
                        lastAssignedBy = query_lecturer?.id == null ? null : userId;
                    }

                    // Declare curriculum class
                    curriculum_class curriculumClass = new curriculum_class()
                    {
                        curriculum_class_id = ToNullableString(curriculumClassid),
                        original_id = ToNullableString(originalId),
                        type = ToNullableString(type),
                        student_class_id = ToNullableString(studentClassId),
                        minimum_student = ToNullableInt(minimumStudent),
                        total_lesson = ToNullableInt(totalLesson),
                        day = ToNullableString(day),
                        start_lesson = ToInt(startLesson),
                        lesson_number = ToInt(lessonNumber),
                        lesson_time = ToNullableString(lessonTime),
                        student_number = ToNullableInt(studentNumber),
                        free_slot = ToNullableInt(freeSlot),
                        state = ToNullableString(state),
                        learn_week = ToNullableString(learnWeek),
                        day_2 = ToInt(day2),
                        start_lesson_2 = ToInt(startLesson2),
                        student_registered_number = ToNullableInt(studentRegisteredNumber),
                        start_week = ToInt(startWeek),
                        end_week = ToInt(endWeek),
                        note_1 = ToNullableString(note1),
                        note_2 = ToNullableString(note2),
                        last_assigned_by = lastAssignedBy,
                        lecturer_id = query_lecturer?.id,
                        term_id = term,
                        major_id = major,
                        curriculum_id = ToNullableString(curriculumId),
                        room_id = ToNullableString(roomId)
                    };

                    if (!isUpdate)
                    {
                        // Create new curriculum class
                        unitOfWork.CurriculumClassRepository.InsertCurriculumClass(curriculumClass);
                    }
                    else
                    {
                        curriculum_class query_curriculumClass = unitOfWork.CurriculumClassRepository.FindCurriculumClass(query_curriculumClassWhere, curriculumClass.curriculum_class_id, curriculumClass.day_2, curriculumClass.room_id);
                        if (query_curriculumClass == null)
                        {
                            // Create new curriculum class if no class found
                            curriculumClass.last_assigned_by = null;
                            curriculumClass.lecturer_id = null;
                            unitOfWork.CurriculumClassRepository.InsertCurriculumClass(curriculumClass);
                        }
                        else if (query_curriculumClass?.lecturer_id == null && curriculumClass.lecturer_id != null)
                        {
                            dynamic checkState = CheckState(query_curriculumClass.id, term, curriculumClass.lecturer_id).Data;
                            if (checkState.success)
                            {
                                // update lecturer if check success
                                query_curriculumClass.last_assigned_by = lastAssignedBy;
                                query_curriculumClass.lecturer_id = curriculumClass.lecturer_id;
                                unitOfWork.Save();
                            }
                            else
                            {
                                // Add lecturer to error list
                                errorAssignList.Add(Tuple.Create(lecturerId, fullName, curriculumClassid, day, lessonTime, checkState.message));
                            }
                        }
                    }
                    ProgressHub.SendProgress("Đang import...", dt.Rows.IndexOf(row), itemsCount);
                }
                unitOfWork.Save();
                TimetableHub.RefreshData(term, major);
                return errorLecturerList.Count > 0
                    ? Json(errorLecturerList.Distinct(), JsonRequestBehavior.AllowGet)
                    : (ActionResult)Json(errorAssignList, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                Response.Write($"Oops, có lỗi đã xảy ra, vui lòng kiểm tra lại tệp tin");
                return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
            }
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

            IEnumerable<curriculum_class> classes;
            if (majorId != "-1")
            {
                // Export data in term and major
                classes = unitOfWork.CurriculumClassRepository.GetExportData(termId, majorId);
            }
            else
            {
                // Export data in whole term and update major name for file
                classes = unitOfWork.CurriculumClassRepository.GetClassesByTerm(termId);
                majorId = "TatCa";
            }

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
        [Authorize(Roles = "BCN khoa, Bộ môn")]
        public ActionResult Assign()
        {
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            ViewData["major"] = new SelectList(unitOfWork.MajorRepository.GetMajors(), "id", "name");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "BCN khoa, Bộ môn")]
        public JsonResult Assign(int id, string lecturerId)
        {
            // Declare variables
            curriculum_class curriculumClass = unitOfWork.CurriculumClassRepository.GetClassByID(id);
            string currentLecturerId = UserManager.FindByEmail(User.Identity.Name).Id;
            lecturerId = ToNullableString(lecturerId);

            // Check if user is in role Department head to restrict assignment
            if (User.IsInRole("Bộ môn") && curriculumClass.last_assigned_by != null && curriculumClass.last_assigned_by != currentLecturerId)
            {
                string lastAssignedBy = unitOfWork.UserRepository.GetLecturerByID(curriculumClass.last_assigned_by).full_name;
                return Json(new { error = true, message = "Lớp học phần này đã được phân công bởi " + lastAssignedBy + "!" }, JsonRequestBehavior.AllowGet);
            }

            // Update lecturer id of curriculum class
            curriculumClass.lecturer_id = lecturerId;
            curriculumClass.last_assigned_by = lecturerId != null ? currentLecturerId : null;
            unitOfWork.Save();

            // Send signal to SignalR Hub
            TimetableHub.BroadcastData(id, lecturerId, curriculumClass.lecturer?.full_name, true);
            return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = "BCN khoa, Bộ môn")]
        public JsonResult CheckState(int id, int termId, string lecturerId)
        {
            if (lecturerId != string.Empty)
            {
                // Declare variables
                curriculum_class curriculumClass = unitOfWork.CurriculumClassRepository.GetClassByID(id);
                term term = unitOfWork.TermRepository.GetTermByID(termId);
                IEnumerable<curriculum_class> query_classWeek = unitOfWork.CurriculumClassRepository.GetClassesInTerm(termId, lecturerId);
                IEnumerable<curriculum_class> query_classDay = unitOfWork.CurriculumClassRepository.GetClassesInDay(query_classWeek, curriculumClass.day_2);
                IEnumerable<curriculum_class> query_classCampus = unitOfWork.CurriculumClassRepository.GetClassesInCampus(query_classDay, curriculumClass.start_lesson_2, curriculumClass.room_id);
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
                        roomId = c.room_id,
                        majorName = c.major.name
                    }).ToList();
                    return Json(new { success = false, message = "Giảng viên này đã có lớp trong tiết học này!", classList = classes }, JsonRequestBehavior.AllowGet);
                }

                // Check previous and next class not in the other campus
                if (query_classCampus.Any())
                {
                    IEnumerable classes = query_classCampus.Select(c => new
                    {
                        classId = c.curriculum_class_id,
                        curriculumName = c.curriculum.name,
                        classDay = c.day,
                        lessonTime = c.lesson_time,
                        roomId = c.room_id,
                        majorName = c.major.name
                    }).ToList();
                    return Json(new { success = false, message = "Giảng viên này đã có lớp ở cơ sở khác!", classList = classes }, JsonRequestBehavior.AllowGet);
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
                        roomId = c.room_id,
                        majorName = c.major.name
                    }).ToList();
                    return Json(new { success = false, message = "Giảng viên này đã đạt số tiết tối đa trong 1 ngày!", classList = classes }, JsonRequestBehavior.AllowGet);
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
                        roomId = c.room_id,
                        majorName = c.major.name
                    }).ToList();
                    return Json(new { success = false, message = "Giảng viên này đạt số lớp tối đa trong 1 tuần!", classList = classes }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "BCN khoa")]
        public ActionResult Delete(int id)
        {
            // Delete class
            unitOfWork.CurriculumClassRepository.DeleteClass(id);
            unitOfWork.Save();

            // Send signal to SignalR Hub
            TimetableHub.BroadcastData(id, null, null, false);
            return Json(new { success = true, message = "Xoá thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "BCN khoa")]
        public ActionResult DeleteAll(int term, string major)
        {
            // Delete all records in curriculum class table
            unitOfWork.CurriculumClassRepository.DeleteAllClasses(term, major);
            unitOfWork.Save();
            TimetableHub.RefreshData(term, major);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
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
        private string ValidateColumns(DataTable dt)
        {
            // Declare the valid column names
            string[] validColumns = {
                "MaGocLHP", "Mã MH", "Mã LHP", "Tên HP", "Số TC", "Loại HP", "Mã Lớp", "TSMH",
                "Số Tiết Đã xếp", "PH", "Thứ", "Tiết BĐ", "Số Tiết", "Tiết Học", "Phòng", "Mã CBGD",
                "Tên CBGD", "PH_X", "Sức Chứa", "SiSoTKB", "Trống", "Tình Trạng LHP", "TuanHoc2", "ThuS",
                "TietS", "Số SVĐK", "Tuần BD", "Tuần KT", "Ghi Chú 1", "Ghi chú 2"
            };

            DataColumnCollection columns = dt.Columns;
            // Validate all columns in excel file
            foreach (string validColumn in validColumns)
            {
                if (!columns.Contains(validColumn))
                {
                    // Return error message
                    return validColumn;
                }
            }
            return null;
        }

        private string ValidateNotNull(string[] validRows)
        {
            foreach (string validRow in validRows)
            {
                // Check if string is null
                if (ToNullableString(validRow) == null)
                {
                    return validRow;
                }
            }
            return null;
        }

        public static string ToNullableString(string value)
        {
            // Check if string is empty
            return value != null && string.IsNullOrEmpty(value.Trim()) ? null : value;
        }

        public static int? ToNullableInt(string value)
        {
            // Convert string to nullable int
            return value != null && string.IsNullOrEmpty(value.Trim()) ? (int?)null : int.Parse(value);
        }

        public static int ToInt(string value)
        {
            // Convert string to int
            return int.Parse(value);
        }
        #endregion
    }
}