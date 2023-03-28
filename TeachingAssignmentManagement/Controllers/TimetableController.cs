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
using TeachingAssignmentManagement.Helpers;
using TeachingAssignmentManagement.Hubs;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Controllers
{
    [Authorize(Roles = CustomRoles.AllRoles)]
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
            IEnumerable<ClassSectionDTO> query_classes = unitOfWork.ClassSectionRepository.GetTimetable(termId, userId);
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
                return PartialView("_PersonalTimetable", new TimetableViewModel
                {
                    ClassSectionDTOs = unitOfWork.ClassSectionRepository.GetClassInWeek(query_classes, currentWeek)
                });
            }
        }

        [HttpGet]
        public ActionResult GetData(int termId, string majorId)
        {
            IEnumerable<ClassSectionDTO> query_classes = majorId != "-1"
                ? unitOfWork.ClassSectionRepository.GetAssignTimetable(termId, majorId)
                : unitOfWork.ClassSectionRepository.GetTermAssignTimetable(termId);
            ViewData["termStatus"] = unitOfWork.TermRepository.GetTermByID(termId).status;
            ViewBag.subjects = unitOfWork.SubjectRepository.GetSubjects(query_classes);
            ViewBag.lecturers = new SelectList(unitOfWork.UserRepository.GetLecturers(), "Id", "FullName");
            return PartialView("_Timetable", new TimetableViewModel
            {
                ClassSectionDTOs = query_classes.ToList()
            });
        }

        [HttpGet]
        [Authorize(Roles = CustomRoles.FacultyBoard)]
        public ActionResult Import()
        {
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            ViewData["major"] = new SelectList(unitOfWork.MajorRepository.GetMajors(), "id", "name");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = CustomRoles.FacultyBoard)]
        public ActionResult Import(HttpPostedFileBase postedFile, int term, string major, bool isUpdate, bool isCheckStudentNumber)
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

                        // Get the name of First Sheet.
                        connExcel.Open();
                        DataTable dtExcelSchema;
                        dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                        connExcel.Close();

                        // Read Data from First Sheet.
                        connExcel.Open();
                        cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                        odaExcel.SelectCommand = cmdExcel;
                        odaExcel.Fill(dt);
                        connExcel.Close();
                    }
                }
            }

            // Check if term status is false to prevent assignments
            bool termStatus = unitOfWork.TermRepository.GetTermByID(term).status;
            if (!termStatus)
            {
                Response.Write($"Học kỳ này đã được khoá phân công trên hệ thống!");
                return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
            }

            // Get current User Id
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
                bool haveData = unitOfWork.ClassSectionRepository.CheckClassesInTermMajor(term, major);
                if (haveData)
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
            TimetableViewModel timetableViewModel = new TimetableViewModel();
            List<class_section> classSectionList = new List<class_section>();
            IEnumerable<class_section> query_classSectionWhere = classSectionList;
            if (isUpdate)
            {
                // Query classes of this term
                query_classSectionWhere = unitOfWork.ClassSectionRepository.GetClassesByTerm(term);
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
                    string subjectId = row["Mã MH"].ToString();
                    string classSectionid = row["Mã LHP"].ToString();
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
                    string[] validRows = { originalId, subjectId, classSectionid, name, credits, type, totalLesson, day, startLesson, lessonNumber, roomId, learnWeek, day2, startLesson2, studentRegisteredNumber, startWeek, endWeek };
                    string checkNull = ValidateNotNull(validRows);
                    if (checkNull != null)
                    {
                        int excelRow = dt.Rows.IndexOf(row) + 2;
                        Response.Write($"Oops, có lỗi đã xảy ra ở dòng số <strong>" + excelRow + "</strong>, vui lòng kiểm tra lại tệp tin.");
                        return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
                    }

                    // Check if student registered number is less than 1
                    if (ToInt(studentRegisteredNumber) <= 0 && isCheckStudentNumber)
                    {
                        Response.Write($"Có một số lớp có sinh viên đăng ký là 0, bạn có muốn import tiếp không?");
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }

                    // Check if start lessons is true
                    if (!timetableViewModel.startLessons.Contains(ToInt(startLesson2)))
                    {
                        int excelRow = dt.Rows.IndexOf(row) + 2;
                        Response.Write($"Oops, có lỗi đã xảy ra ở dòng số <strong>" + excelRow + "</strong>, tiết bắt đầu phải là 1, 4, 7, 10 hoặc 13.");
                        return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
                    }

                    string subjectKey = $"{subjectId}-{term}-{major}";
                    subject query_subject = unitOfWork.SubjectRepository.GetSubjectByID(subjectKey);
                    if (query_subject == null)
                    {
                        // Create new subject
                        subject subject = new subject()
                        {
                            id = subjectKey,
                            subject_id = ToNullableString(subjectId),
                            name = ToNullableString(name),
                            credits = ToInt(credits),
                            is_vietnamese = true,
                            term_id = term,
                            major_id = major
                        };
                        unitOfWork.SubjectRepository.InsertSubject(subject);
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

                    // Declare class
                    class_section classSection = new class_section()
                    {
                        class_section_id = ToNullableString(classSectionid),
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
                        subject_id = subjectKey,
                        room_id = ToNullableString(roomId)
                    };

                    if (!isUpdate)
                    {
                        // Create new class
                        unitOfWork.ClassSectionRepository.InsertClassSection(classSection);
                    }
                    else
                    {
                        class_section query_classSection = unitOfWork.ClassSectionRepository.FindClassSection(query_classSectionWhere, classSection.class_section_id, classSection.day_2, classSection.start_lesson_2, classSection.room_id);
                        query_classSection.student_registered_number = classSection.student_registered_number;
                        if (query_classSection == null)
                        {
                            // Create new class if no class found
                            classSection.last_assigned_by = null;
                            classSection.lecturer_id = null;
                            unitOfWork.ClassSectionRepository.InsertClassSection(classSection);
                        }
                        else if (query_classSection?.lecturer_id == null && classSection.lecturer_id != null)
                        {
                            dynamic checkState = CheckState(query_classSection.id, term, classSection.lecturer_id, true).Data;
                            if (checkState.success)
                            {
                                // update lecturer if check success
                                query_classSection.last_assigned_by = lastAssignedBy;
                                query_classSection.lecturer_id = classSection.lecturer_id;
                                unitOfWork.Save();
                            }
                            else
                            {
                                // Add lecturer to error list
                                errorAssignList.Add(Tuple.Create(lecturerId, fullName, classSectionid, day, lessonTime, checkState.message));
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

            IEnumerable<class_section> classes;
            if (majorId != "-1")
            {
                // Export data in term and major
                classes = unitOfWork.ClassSectionRepository.GetExportData(termId, majorId);
            }
            else
            {
                // Export data in whole term and update major name for file
                classes = unitOfWork.ClassSectionRepository.GetClassesByTerm(termId);
                majorId = "TatCa";
            }

            foreach (class_section item in classes)
            {
                // Add data to table
                dt.Rows.Add(item.original_id, item.subject.subject_id, item.class_section_id, item.subject.name,
                    item.subject.credits, item.type, item.student_class_id, item.minimum_student, item.total_lesson,
                    item.room.room_2, item.day, item.start_lesson, item.lesson_number, item.lesson_time, item.room_id,
                    item.lecturer?.staff_id, item.lecturer?.full_name, item.room.type, item.room.capacity, item.student_number,
                    item.free_slot, item.state, item.learn_week, item.day_2, item.start_lesson_2,
                    item.student_registered_number, item.start_week, item.end_week, item.note_1, item.note_2);
            }

            using (XLWorkbook workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add(dt);
                int excelRow = dt.Rows.Count + 1;
                for (int row = 1; row <= excelRow; row++)
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
        [Authorize(Roles = CustomRoles.FacultyBoardOrDepartment)]
        public ActionResult Assign()
        {
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            ViewData["major"] = new SelectList(unitOfWork.MajorRepository.GetMajors(), "id", "name");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = CustomRoles.FacultyBoardOrDepartment)]
        public JsonResult Assign(int id, string lecturerId)
        {
            // Declare variables
            class_section classSection = unitOfWork.ClassSectionRepository.GetClassByID(id);
            string currentLecturerId = UserManager.FindByEmail(User.Identity.Name).Id;
            string currentLecturerName = unitOfWork.UserRepository.GetLecturerByID(currentLecturerId).full_name;
            lecturerId = ToNullableString(lecturerId);

            // Check if user is in role Department to restrict assignment
            if (User.IsInRole(CustomRoles.Department) && classSection.last_assigned_by != null && classSection.last_assigned_by != currentLecturerId)
            {
                string lastAssignedBy = unitOfWork.UserRepository.GetLecturerByID(classSection.last_assigned_by).full_name;
                return Json(new { error = true, message = "Lớp học phần này đã được phân công bởi " + lastAssignedBy + "!" }, JsonRequestBehavior.AllowGet);
            }

            // Update lecturer id of class
            classSection.lecturer_id = lecturerId;
            classSection.last_assigned_by = lecturerId != null ? currentLecturerId : null;
            unitOfWork.Save();

            // Send signal to SignalR Hub
            TimetableHub.BroadcastData(id, lecturerId, classSection.lecturer?.full_name, currentLecturerName, true);
            return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Authorize(Roles = CustomRoles.FacultyBoardOrDepartment)]
        public JsonResult CheckState(int id, int termId, string lecturerId, bool warning)
        {
            if (lecturerId != string.Empty)
            {
                // Declare variables
                class_section classSection = unitOfWork.ClassSectionRepository.GetClassByID(id);
                term term = unitOfWork.TermRepository.GetTermByID(termId);
                IEnumerable<class_section> query_classWeek = unitOfWork.ClassSectionRepository.GetClassesInTerm(termId, lecturerId);
                IEnumerable<class_section> query_classDay = unitOfWork.ClassSectionRepository.GetClassesInDay(query_classWeek, classSection.day_2);
                IEnumerable<class_section> query_classLesson = unitOfWork.ClassSectionRepository.GetClassesInLesson(query_classDay, classSection.start_lesson_2);
                IEnumerable<class_section> query_classCampus = unitOfWork.ClassSectionRepository.GetClassesInCampus(query_classDay, classSection.start_lesson_2, classSection.room_id);

                // Check not duplicate class in the same time
                if (query_classLesson.Count() >= 1)
                {
                    IEnumerable classes = query_classLesson.Select(c => new
                    {
                        classId = c.class_section_id,
                        subjectName = c.subject.name,
                        classDay = c.day,
                        lessonTime = c.lesson_time,
                        majorName = c.major.name
                    }).ToList();
                    return Json(new { success = false, message = "Giảng viên này đã có lớp trong tiết học này!", classList = classes }, JsonRequestBehavior.AllowGet);
                }

                // Check maximum lessons in a day
                if (query_classDay.Sum(c => c.lesson_number) + classSection.lesson_number > term.max_lesson)
                {
                    IEnumerable classes = query_classDay.Select(c => new
                    {
                        classId = c.class_section_id,
                        subjectName = c.subject.name,
                        classDay = c.day,
                        lessonTime = c.lesson_time,
                        majorName = c.major.name
                    }).ToList();
                    return Json(new { success = false, message = "Giảng viên này đã đạt số tiết tối đa trong 1 ngày!", classList = classes }, JsonRequestBehavior.AllowGet);
                }

                // Check maximum classes in a week
                if (query_classWeek.Count() >= term.max_class)
                {
                    IEnumerable classes = query_classWeek.Select(c => new
                    {
                        classId = c.class_section_id,
                        subjectName = c.subject.name,
                        classDay = c.day,
                        lessonTime = c.lesson_time,
                        majorName = c.major.name
                    }).ToList();
                    return Json(new { success = false, message = "Giảng viên này đạt số lớp tối đa trong 1 tuần!", classList = classes }, JsonRequestBehavior.AllowGet);
                }

                // Check previous and next class not in the other campus
                if (query_classCampus.Any() && warning == true)
                {
                    IEnumerable classes = query_classCampus.Select(c => new
                    {
                        classId = c.class_section_id,
                        subjectName = c.subject.name,
                        classDay = c.day,
                        lessonTime = c.lesson_time,
                        roomId = c.room_id,
                        majorName = c.major.name
                    }).ToList();
                    return Json(new { success = false, warning = true, message = "Giảng viên này đã được phân ca liền kề ở cơ sở khác!", classList = classes }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = CustomRoles.FacultyBoard)]
        public ActionResult Delete(int id)
        {
            // Delete class
            unitOfWork.ClassSectionRepository.DeleteClass(id);
            unitOfWork.Save();

            // Send signal to SignalR Hub
            TimetableHub.BroadcastData(id, null, null, null, false);
            return Json(new { success = true, message = "Xoá thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = CustomRoles.FacultyBoard)]
        public ActionResult DeleteAll(int term, string major)
        {
            // Delete all records in class section table
            unitOfWork.ClassSectionRepository.DeleteAllClasses(term, major);
            unitOfWork.SubjectRepository.DeleteAllSubjects(term, major);
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
        public string ValidateColumns(DataTable dt)
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

        public string ValidateNotNull(string[] validRows)
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

        public string ToNullableString(string value)
        {
            // Check if string is empty
            return value != null && string.IsNullOrEmpty(value.Trim()) ? null : value;
        }

        public int? ToNullableInt(string value)
        {
            // Convert string to nullable int
            return value != null && string.IsNullOrEmpty(value.Trim()) ? (int?)null : int.Parse(value);
        }

        public int ToInt(string value)
        {
            // Convert string to int
            return int.Parse(value);
        }
        #endregion
    }
}