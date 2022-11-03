using System;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Areas.FacultyBoard.Controllers
{
    [Authorize(Roles = "BCN khoa")]
    public class TimetableController : Controller
    {
        private readonly ITermRepository termRepository;
        private readonly IMajorRepository majorRepository;

        public TimetableController()
        {
            this.termRepository = new TermRepository(new CP25Team03Entities());
            this.majorRepository = new MajorRepository(new CP25Team03Entities());
        }

        public TimetableController(ITermRepository termRepository, IMajorRepository majorRepository)
        {
            this.termRepository = termRepository;
            this.majorRepository = majorRepository;
        }

        // GET: FacultyBoard/Timetable
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Import()
        {
            ViewBag.term = new SelectList(termRepository.GetTerms(), "id", "id");
            ViewBag.major = new SelectList(majorRepository.GetMajors(), "id", "name");
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

            // Declare the valid column names
            string[] validColumns = {
                "MaGocLHP", "Mã MH", "Mã LHP", "Tên HP", "Số TC", "Loại HP", "Mã Lớp", "TSMH",
                "Số Tiết Đã xếp", "PH", "Thứ", "Tiết BĐ", "Số Tiết", "Tiết Học", "Phòng", "Mã CBGD",
                "Tên CBGD", "PH_X", "Sức Chứa", "SiSoTKB", "Trống", "Tình Trạng LHP", "TuanHoc2", "ThuS",
                "TietS", "Số SVĐK", "Tuần BD", "Tuần KT", "Ghi Chú 1", "Ghi chú 2"
            };

            // Trim column name string
            foreach (DataColumn col in dt.Columns)
            {
                col.ColumnName = col.ColumnName.Trim();
            }

            // Validate all columns
            string isValid = ValidateColumns(dt, validColumns);
            if (isValid != null)
            {
                Response.Write($"Có vẻ như bạn đã sai tên cột <strong>" + isValid + "</strong>, vui lòng kiểm tra lại tệp tin! 😟");
                return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
            }

            int itemsCount = dt.Rows.Count;

            try
            {
                //Insert records to database table.
                foreach (DataRow row in dt.Rows)
                {
                    // Declare all columns
                    string originalId = row["MaGocLHP"].ToString();
                    string curriculumId = row["Mã MH"].ToString();
                    string id = row["Mã LHP"].ToString();
                    string name = row["Tên HP"].ToString();
                    string credits = row["Số TC"].ToString();
                    string type = row["Loại HP"].ToString();
                    string studentClassId = row["Mã Lớp"].ToString();
                    string minimumStudent = row["TSMH"].ToString();
                    string totalLesson = row["MaGocLHP"].ToString();
                    string room = row["MaGocLHP"].ToString();
                    string day = row["MaGocLHP"].ToString();
                    string startLesson = row["MaGocLHP"].ToString();
                    string lessonNumber = row["MaGocLHP"].ToString();
                    string lessonTime = row["MaGocLHP"].ToString();
                    string room2 = row["MaGocLHP"].ToString();
                    string lecturerId = row["MaGocLHP"].ToString();
                    string fullName = row["MaGocLHP"].ToString();
                    string roomType = row["MaGocLHP"].ToString();
                    string capacity = row["MaGocLHP"].ToString();
                    string studentNumber = row["MaGocLHP"].ToString();
                    string freeSlot = row["MaGocLHP"].ToString();
                    string state = row["MaGocLHP"].ToString();
                    string learnWeek = row["MaGocLHP"].ToString();
                    string day2 = row["MaGocLHP"].ToString();
                    string startLesson2 = row["MaGocLHP"].ToString();
                    string studentRegisteredNumber = row["MaGocLHP"].ToString();
                    string startWeek = row["MaGocLHP"].ToString();
                    string endWeek = row["MaGocLHP"].ToString();
                    string note1 = row["MaGocLHP"].ToString();
                    string note2 = row["MaGocLHP"].ToString();


                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
            }
            return RedirectToAction("Import");
        }

        private string ValidateColumns(DataTable dt, string[] validColumns)
        {
            DataColumnCollection columns = dt.Columns;
            // Validate all columns in excel file
            foreach(string validColumn in validColumns)
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
    }
}