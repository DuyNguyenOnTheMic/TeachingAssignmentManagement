using System.Collections.Generic;
using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Helpers;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Controllers
{
    [Authorize(Roles = CustomRoles.FacultyBoard)]
    public class LecturerRankController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public LecturerRankController()
        {
            unitOfWork = new UnitOfWork(new CP25Team03Entities());
        }

        public LecturerRankController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public ActionResult Index()
        {
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            return View();
        }

        [HttpGet]
        public ActionResult GetLecturerRankPartial(int termId)
        {
            ViewData["termId"] = termId;
            return PartialView("_LecturerRank");
        }

        [HttpGet]
        public JsonResult GetData(int termId)
        {
            // Get lecturer rank data from database
            return Json(unitOfWork.LecturerRankRepository.GetLecturerRanksInTerm(termId), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Create(int termId, string lecturerId)
        {
            ViewData["termId"] = termId;
            ViewData["academic_degree_rank_id"] = new SelectList(unitOfWork.AcademicDegreeRankRepository.GetAcademicDegreeRankDTO(), "Id", "Id");
            return View(unitOfWork.UserRepository.GetLecturerByID(lecturerId));
        }

        [HttpPost]
        public ActionResult Create(int termId, string id, string academic_degree_rank_id)
        {
            // Create new lecturer rank
            bool isLecturerRankExists = unitOfWork.LecturerRankRepository.CheckLecturerRankExists(termId, id);
            if (!isLecturerRankExists)
            {
                lecturer_rank lecturerRank = new lecturer_rank
                {
                    academic_degree_rank_id = academic_degree_rank_id,
                    lecturer_id = id,
                    term_id = termId
                };
                unitOfWork.LecturerRankRepository.InsertLecturerRank(lecturerRank);
                unitOfWork.Save();
                return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { error = true, message = "Giảng viên đã được phân cấp bậc trong học kỳ này, vui lòng thử lại sau!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            lecturer_rank lecturerRank = unitOfWork.LecturerRankRepository.GetLecturerRankByID(id);
            ViewData["academic_degree_rank_id"] = new SelectList(unitOfWork.AcademicDegreeRankRepository.GetAcademicDegreeRankDTO(), "Id", "Id", lecturerRank.academic_degree_rank_id);
            return View(lecturerRank);
        }

        [HttpPost]
        public ActionResult Edit(int id, string academic_degree_rank_id)
        {
            // Update lecturer rank
            lecturer_rank lecturerRank = unitOfWork.LecturerRankRepository.GetLecturerRankByID(id);
            lecturerRank.academic_degree_rank_id = academic_degree_rank_id;
            unitOfWork.Save();
            return Json(new { success = true, message = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditAll(int termId)
        {
            ViewData["termId"] = termId;
            ViewData["academic_degree_rank_id"] = new SelectList(unitOfWork.AcademicDegreeRankRepository.GetAcademicDegreeRankDTO(), "Id", "Id");
            return View();
        }

        [HttpPost]
        public ActionResult EditAll(int termId, string academic_degree_rank_id)
        {
            // Update all lecturer ranks
            unitOfWork.LecturerRankRepository.DeleteAllLecturerRanks(termId);
            IEnumerable<LecturerRankDTO> lecturerRanks = unitOfWork.LecturerRankRepository.GetLecturerRanksInTerm(termId);
            foreach (LecturerRankDTO item in lecturerRanks)
            {
                lecturer_rank lecturerRank = new lecturer_rank
                {
                    academic_degree_rank_id = academic_degree_rank_id,
                    lecturer_id = item.LecturerId,
                    term_id = termId
                };
                unitOfWork.LecturerRankRepository.InsertLecturerRank(lecturerRank);
            }
            unitOfWork.Save();
            return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}