﻿using System;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Areas.FacultyBoard.Controllers
{
    [Authorize(Roles = "BCN khoa")]
    public class TimetableController : Controller
    {
        private readonly ITermRepository termRepository;
        private readonly IMajorRepository majorRepository;
        private readonly ICurriculumClassRepository curriculumClassRepository;

        public TimetableController()
        {
            this.termRepository = new TermRepository(new CP25Team03Entities());
            this.majorRepository = new MajorRepository(new CP25Team03Entities());
            this.curriculumClassRepository = new CurriculumClassRepository(new CP25Team03Entities());
        }

        public TimetableController(ITermRepository termRepository, IMajorRepository majorRepository, ICurriculumClassRepository curriculumClassRepository)
        {
            this.termRepository = termRepository;
            this.majorRepository = majorRepository;
            this.curriculumClassRepository = curriculumClassRepository;
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
                    string originalId = row["MaGocLHP"].ToString();
                    string curriculumId = row["Mã MH"].ToString();
                    string id = row["Mã LHP"].ToString();
                    string name = row["Tên HP"].ToString();
                    string credits = row["Số TC"].ToString();
                    string type = row["Loại HP"].ToString();
                    string studentClassId = row["Mã Lớp"].ToString();
                    string minimumStudent = row["TSMH"].ToString();
                    string totalLesson = row["Số Tiết Đã xếp"].ToString();
                    string room = row["PH"].ToString();
                    string day = row["Thứ"].ToString();
                    string startLesson = row["Tiết BĐ"].ToString();
                    string lessonNumber = row["Số Tiết"].ToString();
                    string lessonTime = row["Tiết Học"].ToString();
                    string room2 = row["Phòng"].ToString();
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
    }
}