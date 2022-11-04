using System;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;

namespace TeachingAssignmentManagement.Areas.FacultyBoard.Controllers
{
    [Authorize(Roles = "BCN khoa")]
    public class TimetableController : Controller
    {
        private readonly UnitOfWork unitOfWork = new UnitOfWork();

        // GET: FacultyBoard/Timetable
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Import()
        {
            ViewBag.term = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            ViewBag.major = new SelectList(unitOfWork.MajorRepository.GetMajors(), "id", "name");
            return View();
        }

        [HttpPost]
        public ActionResult Import(HttpPostedFileBase postedFile, int term, int major)
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

            try
            {
                //Insert records to database table.
                foreach (DataRow row in dt.Rows)
                {
                    // Declare all columns
                    string originalId = ToNullableString(row["MaGocLHP"].ToString());
                    string curriculumId = ToNullableString(row["Mã MH"].ToString());
                    string id = ToNullableString(row["Mã LHP"].ToString());
                    string name = ToNullableString(row["Tên HP"].ToString());
                    int? credits = ToNullableInt(row["Số TC"].ToString());
                    string type = ToNullableString(row["Loại HP"].ToString());
                    string studentClassId = ToNullableString(row["Mã Lớp"].ToString());
                    int? minimumStudent = ToNullableInt(row["TSMH"].ToString());
                    int? totalLesson = ToNullableInt(row["Số Tiết Đã xếp"].ToString());
                    string room = ToNullableString(row["PH"].ToString());
                    string day = ToNullableString(row["Thứ"].ToString());
                    int? startLesson = ToNullableInt(row["Tiết BĐ"].ToString());
                    int? lessonNumber = ToNullableInt(row["Số Tiết"].ToString());
                    string lessonTime = ToNullableString(row["Tiết Học"].ToString());
                    string room2 = ToNullableString(row["Phòng"].ToString());
                    string lecturerId = ToNullableString(row["Mã CBGD"].ToString());
                    string fullName = ToNullableString(row["Tên CBGD"].ToString());
                    string roomType = ToNullableString(row["PH_X"].ToString());
                    int? capacity = ToNullableInt(row["Sức Chứa"].ToString());
                    string studentNumber = ToNullableString(row["SiSoTKB"].ToString());
                    int? freeSlot = ToNullableInt(row["Trống"].ToString());
                    string state = ToNullableString(row["Tình Trạng LHP"].ToString());
                    string learnWeek = ToNullableString(row["TuanHoc2"].ToString());
                    string day2 = ToNullableString(row["ThuS"].ToString());
                    int? startLesson2 = ToNullableInt(row["TietS"].ToString());
                    int? studentRegisteredNumber = ToNullableInt(row["Số SVĐK"].ToString());
                    int? startWeek = ToNullableInt(row["Tuần BD"].ToString());
                    int? endWeek = ToNullableInt(row["Tuần KT"].ToString());
                    string note1 = ToNullableString(row["Ghi Chú 1"].ToString());
                    string note2 = ToNullableString(row["Ghi chú 2"].ToString());


                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
            }
            return RedirectToAction("Import");
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