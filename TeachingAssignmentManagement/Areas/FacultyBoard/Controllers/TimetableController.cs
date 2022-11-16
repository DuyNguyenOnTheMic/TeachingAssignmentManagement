using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Hubs;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Areas.FacultyBoard.Controllers
{
    [Authorize(Roles = "BCN khoa")]
    public class TimetableController : Controller
    {
        private readonly UnitOfWork unitOfWork = new UnitOfWork();

        // GET: FacultyBoard/Timetable
        public ActionResult Index()
        {
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            ViewData["major"] = new SelectList(unitOfWork.MajorRepository.GetMajors(), "id", "name");
            return View(unitOfWork.CurriculumClassRepository.GetCurriculumClasses());
        }

        public JsonResult GetData()
        {
            // Get timetable data from database
            return Json(unitOfWork.CurriculumClassRepository.GetCurriculumClasses(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Import()
        {
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            ViewData["major"] = new SelectList(unitOfWork.MajorRepository.GetMajors(), "id", "name");
            return View();
        }

        [HttpPost]
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
                Response.Write($"Có vẻ như bạn đã sai hoặc thiếu tên cột <strong>" + isValid + "</strong>, vui lòng kiểm tra lại tệp tin! 😟");
                return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
            }

            int itemsCount = dt.Rows.Count;

            List<curriculum_class> curriculumClassList = new List<curriculum_class>();
            IEnumerable<curriculum_class> query_curriculumClassWhere = curriculumClassList;
            if (isUpdate)
            {
                // Query Curriculum classes of this term and major
                query_curriculumClassWhere = unitOfWork.CurriculumClassRepository.GetClassesInTermMajor(term, major);
            }

            // Create a list for storing error Lecturers
            List<Tuple<string, string>> errorLecturerList = new List<Tuple<string, string>>();

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
                    string[] validRows = { originalId, curriculumId, curriculumClassid, name, credits, type, day, startLesson, roomId, day2, startLesson2 };
                    string checkNull = ValidateNotNull(validRows);
                    if (checkNull != null)
                    {
                        int excelRow = dt.Rows.IndexOf(row) + 2;
                        Response.Write($"Oops, có lỗi đã xảy ra ở dòng số <strong>" + excelRow + "</strong>, vui lòng kiểm tra lại tệp tin 🥹");
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
                            credits = (int)ToNullableInt(credits),
                            type = ToNullableString(type)
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

                    lecturer query_lecturer = unitOfWork.UserRepository.GetLecturerByStaffId(lecturerId);
                    if (query_lecturer == null && ToNullableString(lecturerId) != null && ToNullableString(fullName) != null)
                    {
                        // Add record to error list
                        errorLecturerList.Add(Tuple.Create(ToNullableString(lecturerId), ToNullableString(fullName)));
                    }

                    // Declare curriculum class
                    curriculum_class curriculumClass = new curriculum_class()
                    {
                        curriculum_class_id = ToNullableString(curriculumClassid),
                        original_id = ToNullableString(originalId),
                        student_class_id = ToNullableString(studentClassId),
                        minimum_student = ToNullableInt(minimumStudent),
                        total_lesson = ToNullableInt(totalLesson),
                        day = ToNullableString(day),
                        start_lesson = (int)ToNullableInt(startLesson),
                        lesson_number = ToNullableInt(lessonNumber),
                        lesson_time = ToNullableString(lessonTime),
                        student_number = ToNullableInt(studentNumber),
                        free_slot = ToNullableInt(freeSlot),
                        state = ToNullableString(state),
                        learn_week = ToNullableString(learnWeek),
                        day_2 = (int)ToNullableInt(day2),
                        start_lesson_2 = (int)ToNullableInt(startLesson2),
                        student_registered_number = ToNullableInt(studentRegisteredNumber),
                        start_week = ToNullableInt(startWeek),
                        end_week = ToNullableInt(endWeek),
                        note_1 = ToNullableString(note1),
                        note_2 = ToNullableString(note2),
                        term_id = term,
                        major_id = major,
                        lecturer_id = query_lecturer?.id,
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
                        curriculum_class query_curriculumClass = unitOfWork.CurriculumClassRepository.FindCurriculumClass(query_curriculumClassWhere, curriculumClass.curriculum_class_id, curriculumClass.day_2);
                        if (query_curriculumClass?.lecturer_id == null && curriculumClass.lecturer_id != null)
                        {
                            // Update curriculum class's lecturer
                            query_curriculumClass.lecturer_id = curriculumClass.lecturer_id;
                        }
                    }
                    ProgressHub.SendProgress("Đang import...", dt.Rows.IndexOf(row), itemsCount);
                }
                unitOfWork.Save();
                return Json(errorLecturerList.Distinct(), JsonRequestBehavior.AllowGet);
            }
            catch
            {
                Response.Write($"Oops, có lỗi đã xảy ra, vui lòng kiểm tra lại tệp tin 🥹");
                return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
            }
        }

        [HttpPost]
        public ActionResult DeleteAll(int term, string major)
        {
            // Delete all records in curriculum class table
            unitOfWork.CurriculumClassRepository.DeleteAllClasses(term, major);
            unitOfWork.Save();
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

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

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}