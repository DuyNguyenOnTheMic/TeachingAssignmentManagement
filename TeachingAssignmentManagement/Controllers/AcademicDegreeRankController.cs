using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Helpers;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Controllers
{
    [Authorize(Roles = CustomRoles.FacultyBoard)]
    public class AcademicDegreeRankController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public AcademicDegreeRankController()
        {
            unitOfWork = new UnitOfWork(new CP25Team03Entities());
        }

        public AcademicDegreeRankController(UnitOfWork unitOfWork)
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
            // Get Academic, Degree ranks data from database
            return Json(unitOfWork.AcademicDegreeRankRepository.GetAcademicDegreeRanks(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewData["academic_degree_id"] = new SelectList(unitOfWork.AcademicDegreeRepository.GetAcademicDegrees(), "id", "name");
            return View(new academic_degree_rank());
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "id,academic_degree_id")] academic_degree_rank academicDegreeRank)
        {
            try
            {
                // Create new academic degree rank
                unitOfWork.AcademicDegreeRankRepository.InsertAcademicDegreeRank(academicDegreeRank);
                unitOfWork.Save();
            }
            catch
            {
                return Json(new { error = true, message = "Mã cấp bậc này đã tồn tại!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            academic_degree_rank academicDegreeRank = unitOfWork.AcademicDegreeRankRepository.GetAcademicDegreeRankByID(id);
            ViewData["academic_degree_id"] = new SelectList(unitOfWork.AcademicDegreeRepository.GetAcademicDegrees(), "id", "name", academicDegreeRank.academic_degree_id);
            return View(academicDegreeRank);
        }

        [HttpPost]
        public ActionResult Edit([Bind(Include = "id,academic_degree_id")] academic_degree_rank academicDegreeRank)
        {
            // Update academic degree rank
            unitOfWork.AcademicDegreeRankRepository.UpdateAcademicDegreeRank(academicDegreeRank);
            unitOfWork.Save();
            return Json(new { success = true, message = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            try
            {
                // Delete academic degree rank
                unitOfWork.AcademicDegreeRankRepository.DeleteAcademicDegreeRank(id);
                unitOfWork.Save();
            }
            catch
            {
                return Json(new { error = true, message = "Không thể xoá do cấp bậc này đã có dữ liệu!" }, JsonRequestBehavior.AllowGet);
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