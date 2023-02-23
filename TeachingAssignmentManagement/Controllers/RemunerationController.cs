using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            return View();
        }

        [HttpGet]
        public ActionResult GetRemunerationPartial(int termId)
        {
            ViewData["termId"] = termId;
            return PartialView("_Remuneration");
        }

        [HttpGet]
        public JsonResult GetRemunerationData(int termId)
        {
            // Declare variables
            IEnumerable<LecturerRankDTO> lecturerRanks = unitOfWork.LecturerRankRepository.GetLecturerRanksInTerm(termId);
            term term = unitOfWork.TermRepository.GetTermByID(termId);
            int startYear = term.start_year;
            int endYear = term.end_year;
            coefficient coefficient = unitOfWork.CoefficientRepository.GetCoefficientInYear(startYear, endYear);
            IEnumerable<unit_price> unitPrice = unitOfWork.UnitPriceRepository.GetUnitPriceInYear(startYear, endYear);
            List<RemunerationDTO> remunerationDTOs = new List<RemunerationDTO>();
            bool isMissing = coefficient == null;

            foreach (LecturerRankDTO rank in lecturerRanks)
            {
                // Reset values in each loop
                decimal teachingRemuneration = decimal.Zero;
                isMissing = false;

                // Check if lecturer have been assigned a rank
                if (rank.Id != null)
                {
                    decimal unitPriceByLevel;

                    // Get unit price for lecturer rank
                    unit_price query_unitPrice = unitPrice.SingleOrDefault(u => u.academic_degree_rank_id == rank.AcademicDegreeRankId && u.type == Constants.StandardProgramType);
                    if (query_unitPrice != null)
                    {
                        unitPriceByLevel = query_unitPrice.unit_price1;
                    }
                    else
                    {
                        isMissing = true;
                        unitPriceByLevel = decimal.Zero;
                    }

                    // Get classes in term of lecturer
                    IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetPersonalClassesInTerm(termId, rank.LecturerId);
                    foreach (class_section item in query_classes)
                    {
                        teachingRemuneration += CalculateRemuneration(item, unitPriceByLevel, coefficient);
                    }
                }
                remunerationDTOs.Add(new RemunerationDTO
                {
                    StaffId = rank.StaffId,
                    FullName = rank.FullName,
                    AcademicDegreeRankId = rank.AcademicDegreeRankId,
                    Remuneration = teachingRemuneration,
                    Status = isMissing
                });
            }
            return Json(remunerationDTOs, JsonRequestBehavior.AllowGet);
        }

        public static decimal CalculateRemuneration(class_section classSection, decimal unitPriceByLevel, coefficient coefficient)
        {
            decimal crowdedClassCoefficient, timeCoefficient, languageCoefficient, classTypeCoefficient;

            // Check if class is theoretical or practice
            int studentNumber;
            if (classSection.type == Constants.TheoreticalClassType)
            {
                studentNumber = 50;
                classTypeCoefficient = coefficient.theoretical_coefficient;
            }
            else
            {
                studentNumber = 30;
                classTypeCoefficient = coefficient.practice_coefficient;
            }

            // Calculate crowded class coefficient
            int? studentRegistered = classSection.student_registered_number;
            crowdedClassCoefficient = studentRegistered <= studentNumber ? decimal.One : (decimal)(decimal.One + (studentRegistered - studentNumber) * 0.0025m);

            // Calculate time coefficient
            timeCoefficient = classSection.start_lesson_2 != 13 ? decimal.One : 1.2m;

            // Calculate language coefficient
            languageCoefficient = classSection.subject.is_vietnamese ? coefficient.vietnamese_coefficient : coefficient.foreign_coefficient;

            // Calculate total remuneration for this class
            return unitPriceByLevel * crowdedClassCoefficient * timeCoefficient * classTypeCoefficient * languageCoefficient;
        }

        [HttpGet]
        public ActionResult LecturerRank()
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
        public JsonResult GetLecturerRankData(int termId)
        {
            // Get lecturer rank data from database
            return Json(unitOfWork.LecturerRankRepository.GetLecturerRanksInTerm(termId), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CreateLecturerRank(int termId, string lecturerId)
        {
            ViewData["termId"] = termId;
            ViewData["academic_degree_rank_id"] = new SelectList(unitOfWork.AcademicDegreeRankRepository.GetAcademicDegreeRankDTO(), "Id", "Id");
            return View(unitOfWork.UserRepository.GetLecturerByID(lecturerId));
        }

        [HttpPost]
        public ActionResult CreateLecturerRank(int termId, string id, string academic_degree_rank_id)
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
        public ActionResult EditLecturerRank(int id)
        {
            lecturer_rank lecturerRank = unitOfWork.LecturerRankRepository.GetLecturerRankByID(id);
            ViewData["academic_degree_rank_id"] = new SelectList(unitOfWork.AcademicDegreeRankRepository.GetAcademicDegreeRankDTO(), "Id", "Id", lecturerRank.academic_degree_rank_id);
            return View(lecturerRank);
        }

        [HttpPost]
        public ActionResult EditLecturerRank(int id, string academic_degree_rank_id)
        {
            // Update lecturer rank
            lecturer_rank lecturerRank = unitOfWork.LecturerRankRepository.GetLecturerRankByID(id);
            lecturerRank.academic_degree_rank_id = academic_degree_rank_id;
            unitOfWork.Save();
            return Json(new { success = true, message = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditAllLecturerRanks(int termId)
        {
            ViewData["termId"] = termId;
            ViewData["academic_degree_rank_id"] = new SelectList(unitOfWork.AcademicDegreeRankRepository.GetAcademicDegreeRankDTO(), "Id", "Id");
            return View();
        }

        [HttpPost]
        public ActionResult EditAllLecturerRanks(int termId, string academic_degree_rank_id)
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