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
            string filePath = string.Empty;
            string path = Server.MapPath("~/Uploads/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            filePath = path + Path.GetFileName(postedFile.FileName);
            string extension = Path.GetExtension(postedFile.FileName);
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

            // Declare the original column names
            string[] columnNames = {
                "MaGocLHP", "Mã MH", "Mã LHP", "Tên HP", "Số TC", "Loại HP", "Mã Lớp", "TSMH",
                "Số Tiết Đã xếp", "PH", "Thứ", "Tiết BĐ", "Số Tiết", "Tiết Học", "Phòng", "Mã CBGD",
                "Tên CBGD", "PH_X", "Sức Chứa", "SiSoTKB", "Trống", "Tình Trạng LHP", "TuanHoc2", "ThuS",
                "TietS", "Số SVĐK", "Tuần BD", "Tuần KT", "Ghi Chú"
            };

            // Trim column name string
            foreach (DataColumn col in dt.Columns)
            {
                col.ColumnName = col.ColumnName.Trim();
            }

            // Validate all columns
            string isValid = ValidateColumns(dt, columnNames);
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
                    string knowledgeTypeAlias = row["Mã loại kiến thức"].ToString();
                    string knowledgeTypeName = row["Tên loại kiến thức"].ToString();
                    string compulsoryCredits = row["Số chỉ BB"].ToString();
                    string optionalCredits = row["Số chỉ TC"].ToString();
                    string curriculumId = row["Mã học phần"].ToString();
                    string curriculumName = row["Tên học phần (Tiếng Việt)"].ToString();
                    string curriculumNameEnglish = row["Tên học phần (Tiếng Anh)"].ToString();
                    string credits = row["TC"].ToString();
                    string theoreticalHours = row["LT"].ToString();
                    string practiceHours = row["TH"].ToString();
                    string internshipHours = row["TT"].ToString();
                    string projectHours = row["DA"].ToString();
                    string compulsoryOrOptional = row["Bắt buộc/ Tự chọn"].ToString();
                    string prerequisites = row["Điều kiện tiên quyết"].ToString();
                    string learnBefore = row["Học trước – học sau"].ToString();
                    string editingNotes = row["Ghi chú chỉnh sửa"].ToString();



                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.ExpectationFailed);
            }
            return RedirectToAction("Import");
        }

        private string ValidateColumns(DataTable dt, string[] columnNames)
        {
            DataColumnCollection columns = dt.Columns;
            // Validate all columns in excel file
            foreach(string columnName in columnNames)
            {
                if (!columns.Contains(columnName))
                {
                    // Return error message
                    return columnName;
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