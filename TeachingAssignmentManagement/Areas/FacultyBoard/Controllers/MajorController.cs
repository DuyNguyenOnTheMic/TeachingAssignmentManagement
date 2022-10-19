using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Hubs;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Areas.FacultyBoard.Controllers
{
    [Authorize]
    public class MajorController : Controller
    {
        private readonly IMajorRepository majorRepository;

        public MajorController()
        {
            this.majorRepository = new MajorRepository(new CP25Team03Entities());
        }

        public MajorController(IMajorRepository majorRepository)
        {
            this.majorRepository = majorRepository;
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetData()
        {
            // Get majors data from datatabse
            return Json(majorRepository.GetMajors(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
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
                majorRepository.InsertMajor(major);
                majorRepository.Save();
                MajorHub.BroadcastData();
                return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { error = true }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            return View(majorRepository.GetMajorByID(id));
        }

        [HttpPost]
        public ActionResult Edit(major major)
        {
            // Update major
            majorRepository.UpdateMajor(major);
            majorRepository.Save();
            MajorHub.BroadcastData();
            return Json(new { success = true, message = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                // Delete major
                majorRepository.DeleteMajor(id);
                majorRepository.Save();
                MajorHub.BroadcastData();
            }
            catch
            {
                return Json(new { error = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, message = "Xoá thành công!" }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            majorRepository.Dispose();
            base.Dispose(disposing);
        }
    }
}