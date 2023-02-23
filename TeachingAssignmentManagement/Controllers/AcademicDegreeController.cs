using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Controllers
{
    [Authorize(Roles = "BCN khoa")]
    public class AcademicDegreeController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public AcademicDegreeController()
        {
            unitOfWork = new UnitOfWork(new CP25Team03Entities());
        }

        public AcademicDegreeController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [OutputCache(Duration = 600, VaryByCustom = "userName")]
        public ActionResult AcademicDegree()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetData()
        {
            // Get Academics, Degrees data from database
            return Json(unitOfWork.AcademicDegreeRepository.GetAcademicDegrees(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(Duration = 600, VaryByParam = "none")]
        public ActionResult Create()
        {
            return View(new academic_degree());
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "id,name,level")] academic_degree academicDegree)
        {
            try
            {
                // Create new academic degree
                unitOfWork.AcademicDegreeRepository.InsertAcademicDegree(academicDegree);
                unitOfWork.Save();
            }
            catch
            {
                return Json(new { error = true, message = "Mã học hàm, học vị này đã tồn tại!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            return View(unitOfWork.AcademicDegreeRepository.GetAcademicDegreeByID(id));
        }

        [HttpPost]
        public ActionResult Edit([Bind(Include = "id,name,level")] academic_degree academicDegree)
        {
            // Update academic degree
            unitOfWork.AcademicDegreeRepository.UpdateAcademicDegree(academicDegree);
            unitOfWork.Save();
            return Json(new { success = true, message = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                // Delete academic degree
                unitOfWork.AcademicDegreeRepository.DeleteAcademicDegree(id);
                unitOfWork.Save();
            }
            catch
            {
                return Json(new { error = true, message = "Không thể xoá do học hàm, học vị này đã có dữ liệu!" }, JsonRequestBehavior.AllowGet);
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