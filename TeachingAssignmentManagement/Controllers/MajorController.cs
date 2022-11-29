using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Hubs;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Controllers
{
    [Authorize(Roles = "BCN khoa")]
    public class MajorController : Controller
    {
        private readonly UnitOfWork unitOfWork = new UnitOfWork();

        // GET: FacultyBoard/Major
        [OutputCache(Duration = 600, VaryByCustom = "userName")]
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetData()
        {
            // Get majors data from database
            return Json(unitOfWork.MajorRepository.GetMajors(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(Duration = 600, VaryByParam = "none")]
        public ActionResult Create()
        {
            return View(new major());
        }

        [HttpPost]
        public ActionResult Create(major major)
        {
            try
            {
                // Create new major
                unitOfWork.MajorRepository.InsertMajor(major);
                unitOfWork.Save();
            }
            catch
            {
                return Json(new { error = true }, JsonRequestBehavior.AllowGet);
            }
            MajorHub.BroadcastData();
            return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            return View(unitOfWork.MajorRepository.GetMajorByID(id));
        }

        [HttpPost]
        public ActionResult Edit(major major)
        {
            // Update major
            unitOfWork.MajorRepository.UpdateMajor(major);
            unitOfWork.Save();
            MajorHub.BroadcastData();
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
            MajorHub.BroadcastData();
            return Json(new { success = true, message = "Xoá thành công!" }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}