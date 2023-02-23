using System.Collections.Generic;
using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Helpers;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Controllers
{
    [Authorize(Roles = "BCN khoa")]
    public class MajorController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public MajorController()
        {
            unitOfWork = new UnitOfWork(new CP25Team03Entities());
        }

        public MajorController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        [OutputCache(Duration = 600, VaryByCustom = "userName")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetData()
        {
            // Get majors data from database
            return Json(unitOfWork.MajorRepository.GetMajors(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(Duration = 600, VaryByParam = "none")]
        public ActionResult Create()
        {
            ViewData["program_type"] = new SelectList(new Dictionary<int, string>
            {
                { Constants.StandardProgramType, "Tiêu chuẩn" },
                { Constants.SpecialProgramType, "Đặc biệt" }
            }, "Key", "Value");
            return View(new major());
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "id,name,abbreviation,program_type")] major major)
        {
            try
            {
                // Create new major
                unitOfWork.MajorRepository.InsertMajor(major);
                unitOfWork.Save();
            }
            catch
            {
                return Json(new { error = true, message = "Mã ngành này đã tồn tại!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            major major = unitOfWork.MajorRepository.GetMajorByID(id);
            ViewData["program_type"] = new SelectList(new Dictionary<int, string>
            {
                { Constants.StandardProgramType, "Tiêu chuẩn" },
                { Constants.SpecialProgramType, "Đặc biệt" }
            }, "Key", "Value", major.program_type);
            return View(major);
        }

        [HttpPost]
        public ActionResult Edit([Bind(Include = "id,name,abbreviation,program_type")] major major)
        {
            // Update major
            unitOfWork.MajorRepository.UpdateMajor(major);
            unitOfWork.Save();
            return Json(new { success = true, message = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                // Delete major
                unitOfWork.MajorRepository.DeleteMajor(id);
                unitOfWork.Save();
            }
            catch
            {
                return Json(new { error = true, message = "Không thể xoá do ngành này đã có dữ liệu!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, message = "Xoá thành công!" }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}