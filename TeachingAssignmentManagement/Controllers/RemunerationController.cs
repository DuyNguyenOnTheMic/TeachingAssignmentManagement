using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Helpers;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Controllers
{
    [Authorize(Roles = "BCN khoa, Bộ môn")]
    public class RemunerationController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public RemunerationController()
        {
            unitOfWork = new UnitOfWork(new CP25Team03Entities());
        }

        public RemunerationController(UnitOfWork unitOfWork)
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
        public JsonResult GetAcademicDegreeData()
        {
            // Get Academics, Degrees data from database
            return Json(unitOfWork.AcademicDegreeRepository.GetAcademicDegrees(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(Duration = 600, VaryByParam = "none")]
        public ActionResult CreateAcademicDegree()
        {
            return View(new academic_degree());
        }

        [HttpPost]
        public ActionResult CreateAcademicDegree([Bind(Include = "id,name,level")] academic_degree academicDegree)
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
        public ActionResult EditAcademicDegree(string id)
        {
            return View(unitOfWork.AcademicDegreeRepository.GetAcademicDegreeByID(id));
        }

        [HttpPost]
        public ActionResult EditAcademicDegree([Bind(Include = "id,name,level")] academic_degree academicDegree)
        {
            // Update academic degree
            unitOfWork.AcademicDegreeRepository.UpdateAcademicDegree(academicDegree);
            unitOfWork.Save();
            return Json(new { success = true, message = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteAcademicDegree(string id)
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

        [HttpGet]
        [OutputCache(Duration = 600, VaryByCustom = "userName")]
        public ActionResult AcademicDegreeRank()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetAcademicDegreeRankData()
        {
            // Get Academic, Degree ranks data from database
            return Json(unitOfWork.AcademicDegreeRankRepository.GetAcademicDegreeRanks(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CreateAcademicDegreeRank()
        {
            ViewData["academic_degree_id"] = new SelectList(unitOfWork.AcademicDegreeRepository.GetAcademicDegrees(), "id", "name");
            return View(new academic_degree_rank());
        }

        [HttpPost]
        public ActionResult CreateAcademicDegreeRank([Bind(Include = "id,academic_degree_id")] academic_degree_rank academicDegreeRank)
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
        public ActionResult EditAcademicDegreeRank(string id)
        {
            academic_degree_rank academicDegreeRank = unitOfWork.AcademicDegreeRankRepository.GetAcademicDegreeRankByID(id);
            ViewData["academic_degree_id"] = new SelectList(unitOfWork.AcademicDegreeRepository.GetAcademicDegrees(), "id", "name", academicDegreeRank.academic_degree_id);
            return View(academicDegreeRank);
        }

        [HttpPost]
        public ActionResult EditAcademicDegreeRank([Bind(Include = "id,academic_degree_id")] academic_degree_rank academicDegreeRank)
        {
            // Update academic degree rank
            unitOfWork.AcademicDegreeRankRepository.UpdateAcademicDegreeRank(academicDegreeRank);
            unitOfWork.Save();
            return Json(new { success = true, message = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteAcademicDegreeRank(string id)
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

        [HttpGet]
        public ActionResult RankCoefficient()
        {
            ViewData["year"] = new SelectList(unitOfWork.TermRepository.GetYears(), "schoolyear", "schoolyear");
            return View();
        }

        [HttpGet]
        public ActionResult GetRankCoefficientData(int startYear, int endYear)
        {
            // Get ranks data from database
            IEnumerable<rank_coefficient> query_rankCoefficients = unitOfWork.RankCoefficientRepository.GetRankCoefficients(startYear, endYear);
            return PartialView("_RankCoefficient", new RankViewModels
            {
                StartYear = startYear,
                EndYear = endYear,
                AcademicDegreeRankDTOs = unitOfWork.AcademicDegreeRankRepository.GetAcademicDegreeRankDTO(),
                StandardProgramDTOs = unitOfWork.RankCoefficientRepository.GetPrograms(query_rankCoefficients, Constants.StandardProgramType),
                SpecialProgramDTOs = unitOfWork.RankCoefficientRepository.GetPrograms(query_rankCoefficients, Constants.SpecialProgramType),
                ForeignDTOs = unitOfWork.RankCoefficientRepository.GetPrograms(query_rankCoefficients, Constants.ForeignType)
            });
        }

        [HttpGet]
        public ActionResult CreateRankCoefficient(string rankId, int type, int startYear, int endYear)
        {
            ViewData["academic_degree_rank_id"] = rankId;
            ViewData["type"] = type;
            ViewData["start_year"] = startYear;
            ViewData["end_year"] = endYear;
            return View(new rank_coefficient());
        }

        [HttpPost]
        public ActionResult CreateRankCoefficient([Bind(Include = "type,start_year,end_year,academic_degree_rank_id")] rank_coefficient rankCoefficient, string unit_price, string vietnamese_coefficient, string foreign_coefficient)
        {
            // Create new rank coefficient
            bool isRankCoefficientExists = unitOfWork.RankCoefficientRepository.CheckRankCoefficientExists(rankCoefficient.type, rankCoefficient.start_year, rankCoefficient.end_year, rankCoefficient.academic_degree_rank_id);
            if (!isRankCoefficientExists)
            {
                rankCoefficient.unit_price = decimal.Parse(unit_price, CultureInfo.InvariantCulture);
                rankCoefficient.vietnamese_coefficient = decimal.Parse(vietnamese_coefficient, CultureInfo.InvariantCulture);
                rankCoefficient.foreign_coefficient = decimal.Parse(foreign_coefficient, CultureInfo.InvariantCulture);
                unitOfWork.RankCoefficientRepository.InsertRankCoefficient(rankCoefficient);
                unitOfWork.Save();
                return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { error = true, message = "Đã xảy ra lỗi, vui lòng thử lại sau!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditRankCoefficient(int id)
        {
            return View(unitOfWork.RankCoefficientRepository.GetRankCoefficientByID(id));
        }

        [HttpPost]
        public ActionResult EditRankCoefficient(int id, string unit_price, string vietnamese_coefficient, string foreign_coefficient)
        {
            // Update rank coefficient
            rank_coefficient query_rankCoefficient = unitOfWork.RankCoefficientRepository.GetRankCoefficientByID(id);
            query_rankCoefficient.unit_price = decimal.Parse(unit_price, CultureInfo.InvariantCulture);
            query_rankCoefficient.vietnamese_coefficient = decimal.Parse(vietnamese_coefficient, CultureInfo.InvariantCulture);
            query_rankCoefficient.foreign_coefficient = decimal.Parse(foreign_coefficient, CultureInfo.InvariantCulture);
            unitOfWork.Save();
            return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditAllRankCoefficients(int type, int startYear, int endYear)
        {
            ViewData["type"] = type;
            ViewData["startYear"] = startYear;
            ViewData["endYear"] = endYear;
            return View();
        }

        [HttpPost]
        public ActionResult EditAllRankCoefficients(int type, int startYear, int endYear, string unit_price, string vietnamese_coefficient, string foreign_coefficient)
        {
            // Update all rank coefficients
            unitOfWork.RankCoefficientRepository.DeleteAllRankCoefficients(type, startYear, endYear);
            IEnumerable<AcademicDegreeRankDTO> academicDegreeRankDTOs = unitOfWork.AcademicDegreeRankRepository.GetAcademicDegreeRankDTO();
            foreach (AcademicDegreeRankDTO item in academicDegreeRankDTOs)
            {
                rank_coefficient rankCoefficient = new rank_coefficient
                {
                    type = type,
                    unit_price = decimal.Parse(unit_price, CultureInfo.InvariantCulture),
                    vietnamese_coefficient = decimal.Parse(vietnamese_coefficient, CultureInfo.InvariantCulture),
                    foreign_coefficient = decimal.Parse(foreign_coefficient, CultureInfo.InvariantCulture),
                    start_year = startYear,
                    end_year = endYear,
                    academic_degree_rank_id = item.Id
                };
                unitOfWork.RankCoefficientRepository.InsertRankCoefficient(rankCoefficient);
            }
            unitOfWork.Save();
            return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult SubjectCoefficient()
        {
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            ViewData["major"] = new SelectList(unitOfWork.MajorRepository.GetMajors(), "id", "name");
            return View();
        }

        [HttpGet]
        public ActionResult GetSubjectCoefficientPartial(int termId, string majorId)
        {
            ViewData["termId"] = termId;
            ViewData["majorId"] = majorId;
            return PartialView("_SubjectCoefficient");
        }

        [HttpGet]
        public JsonResult GetSubjectCoefficientData(int termId, string majorId)
        {
            // Get subjects data from database
            IEnumerable query_subjects = majorId != "-1"
                ? unitOfWork.SubjectRepository.GetSubjects(termId, majorId)
                : unitOfWork.SubjectRepository.GetTermSubjects(termId);
            return Json(query_subjects, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditSubjectCoefficient(string id)
        {
            return View(unitOfWork.SubjectRepository.GetSubjectByID(id));
        }

        [HttpPost]
        public ActionResult EditSubjectCoefficient(string id, bool is_vietnamese, string theoretical_coefficient, string practice_coefficient)
        {
            // Update subject
            subject query_subject = unitOfWork.SubjectRepository.GetSubjectByID(id);
            query_subject.is_vietnamese = is_vietnamese;
            query_subject.theoretical_coefficient = decimal.Parse(theoretical_coefficient, CultureInfo.InvariantCulture);
            query_subject.practice_coefficient = decimal.Parse(practice_coefficient, CultureInfo.InvariantCulture);
            unitOfWork.Save();
            return Json(new { success = true, message = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditAllSubjectCoefficients(int termId, string majorId)
        {
            ViewData["termId"] = termId;
            ViewData["majorId"] = majorId;
            return View();
        }

        [HttpPost]
        public ActionResult EditAllSubjectCoefficients(int termId, string majorId, string theoretical_coefficient, string practice_coefficient)
        {
            // Update all subjects
            unitOfWork.SubjectRepository.EditAllSubjects(termId, majorId, theoretical_coefficient, practice_coefficient);
            unitOfWork.Save();
            return Json(new { success = true, message = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult LecturerRank()
        {
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}