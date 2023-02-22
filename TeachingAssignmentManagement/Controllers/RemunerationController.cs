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
            termId = 223;
            IEnumerable<LecturerRankDTO> lecturerRanks = unitOfWork.LecturerRankRepository.GetLecturerRanksInTerm(termId);
            term term = unitOfWork.TermRepository.GetTermByID(termId);
            int startYear = term.start_year;
            int endYear = term.end_year;
            coefficient coefficient = unitOfWork.CoefficientRepository.GetCoefficientInYear(startYear, endYear);
            IEnumerable<unit_price> unitPrice = unitOfWork.UnitPriceRepository.GetUnitPriceInYear(startYear, endYear);
            foreach (LecturerRankDTO rank in lecturerRanks)
            {
                // Check if lecturer have been assigned a rank
                if (rank.Id != null)
                {
                    decimal teachingRemuneration = decimal.Zero, unitPriceByLevel, crowdedClassCoefficient, timeCoefficient, languageCoefficient, classTypeCoefficient;

                    // Get classes in term of lecturer
                    IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetPersonalClassesInTerm(termId, rank.LecturerId);
                    foreach (class_section item in query_classes)
                    {
                        // Get unit price for lecturer rank
                        unitPriceByLevel = unitPrice.SingleOrDefault(u => u.academic_degree_rank_id == rank.AcademicDegreeRankId && u.type == Constants.StandardProgramType).unit_price1;

                        // Check if class is theoretical or practice
                        int studentNumber;
                        if (item.type == Constants.TheoreticalClassType)
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
                        int? studentRegistered = item.student_registered_number;
                        if (studentRegistered <= studentNumber)
                        {
                            crowdedClassCoefficient = decimal.One;
                        }
                        else
                        {
                            crowdedClassCoefficient = (decimal)(decimal.One + (studentRegistered - studentNumber) * 0.0025m);
                        }

                        // Calculate time coefficient
                        timeCoefficient = item.start_lesson_2 != 13 ? decimal.One : 1.2m;

                        // Calculate language coefficient
                        languageCoefficient = item.subject.is_vietnamese ? coefficient.vietnamese_coefficient : coefficient.foreign_coefficient;

                        // Calculate total remuneration for this class
                        teachingRemuneration += unitPriceByLevel * crowdedClassCoefficient * timeCoefficient * classTypeCoefficient * languageCoefficient;
                    }

                    var hehe = teachingRemuneration;
                }
            }
            return Json("_Remuneration");
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
        public ActionResult PriceCoefficient()
        {
            ViewData["year"] = new SelectList(unitOfWork.TermRepository.GetYears(), "schoolyear", "schoolyear");
            return View();
        }

        [HttpGet]
        public ActionResult GetPriceCoefficientData(int startYear, int endYear)
        {
            // Get unit price and coefficient data from database
            IEnumerable<unit_price> query_unitPrice = unitOfWork.UnitPriceRepository.GetUnitPriceInYear(startYear, endYear);
            return PartialView("_PriceCoefficient", new PriceCoefficientViewModels
            {
                StartYear = startYear,
                EndYear = endYear,
                AcademicDegreeRankDTOs = unitOfWork.AcademicDegreeRankRepository.GetAcademicDegreeRankDTO(),
                StandardProgramDTOs = unitOfWork.UnitPriceRepository.GetUnitPriceByProgram(query_unitPrice, Constants.StandardProgramType),
                SpecialProgramDTOs = unitOfWork.UnitPriceRepository.GetUnitPriceByProgram(query_unitPrice, Constants.SpecialProgramType),
                ForeignDTOs = unitOfWork.UnitPriceRepository.GetUnitPriceByProgram(query_unitPrice, Constants.ForeignType),
                Coefficients = unitOfWork.CoefficientRepository.GetCoefficientInYear(startYear, endYear)
            });
        }

        [HttpGet]
        public ActionResult CreateUnitPrice(string rankId, int type, int startYear, int endYear)
        {
            ViewData["academic_degree_rank_id"] = rankId;
            ViewData["type"] = type;
            ViewData["start_year"] = startYear;
            ViewData["end_year"] = endYear;
            return View();
        }

        [HttpPost]
        public ActionResult CreateUnitPrice([Bind(Include = "type,start_year,end_year,academic_degree_rank_id")] unit_price unitPrice, string price)
        {
            // Create new unit price
            bool isUnitPriceExists = unitOfWork.UnitPriceRepository.CheckUnitPriceExists(unitPrice.type, unitPrice.start_year, unitPrice.end_year, unitPrice.academic_degree_rank_id);
            if (!isUnitPriceExists)
            {
                unitPrice.unit_price1 = decimal.Parse(price, CultureInfo.InvariantCulture);
                unitOfWork.UnitPriceRepository.InsertUnitPrice(unitPrice);
                unitOfWork.Save();
                return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { error = true, message = "Đơn giá cho cấp bậc đã được đặt trong năm học này, vui lòng thử lại sau!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditUnitPrice(int id)
        {
            return View(unitOfWork.UnitPriceRepository.GetUnitPriceByID(id));
        }

        [HttpPost]
        public ActionResult EditUnitPrice(int id, string price)
        {
            // Update unit price
            unit_price query_unitPrice = unitOfWork.UnitPriceRepository.GetUnitPriceByID(id);
            query_unitPrice.unit_price1 = decimal.Parse(price, CultureInfo.InvariantCulture);
            unitOfWork.Save();
            return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditAllUnitPrice(int type, int startYear, int endYear)
        {
            ViewData["type"] = type;
            ViewData["startYear"] = startYear;
            ViewData["endYear"] = endYear;
            return View();
        }

        [HttpPost]
        public ActionResult EditAllUnitPrice(int type, int startYear, int endYear, string price)
        {
            // Update all unit price
            unitOfWork.UnitPriceRepository.DeleteAllUnitPrice(type, startYear, endYear);
            IEnumerable<AcademicDegreeRankDTO> academicDegreeRankDTOs = unitOfWork.AcademicDegreeRankRepository.GetAcademicDegreeRankDTO();
            foreach (AcademicDegreeRankDTO item in academicDegreeRankDTOs)
            {
                unit_price unitPrice = new unit_price
                {
                    type = type,
                    unit_price1 = decimal.Parse(price, CultureInfo.InvariantCulture),
                    start_year = startYear,
                    end_year = endYear,
                    academic_degree_rank_id = item.Id
                };
                unitOfWork.UnitPriceRepository.InsertUnitPrice(unitPrice);
            }
            unitOfWork.Save();
            return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CreateCoefficient(int startYear, int endYear)
        {
            ViewData["start_year"] = startYear;
            ViewData["end_year"] = endYear;
            return View();
        }

        [HttpPost]
        public ActionResult CreateCoefficient([Bind(Include = "start_year,end_year")] coefficient coefficient, string vietnamese_coefficient, string foreign_coefficient, string theoretical_coefficient, string practice_coefficient)
        {
            // Create new coefficient
            bool isCoefficientExists = unitOfWork.CoefficientRepository.CheckCoefficientExists(coefficient.start_year, coefficient.end_year);
            if (!isCoefficientExists)
            {
                coefficient.vietnamese_coefficient = decimal.Parse(vietnamese_coefficient, CultureInfo.InvariantCulture);
                coefficient.foreign_coefficient = decimal.Parse(foreign_coefficient, CultureInfo.InvariantCulture);
                coefficient.theoretical_coefficient = decimal.Parse(theoretical_coefficient, CultureInfo.InvariantCulture);
                coefficient.practice_coefficient = decimal.Parse(practice_coefficient, CultureInfo.InvariantCulture);
                unitOfWork.CoefficientRepository.InsertCoefficient(coefficient);
                unitOfWork.Save();
                return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { error = true, message = "Hệ số cho cấp bậc đã được đặt trong năm học này, vui lòng thử lại sau!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditCoefficient(int id)
        {
            return View(unitOfWork.CoefficientRepository.GetCoefficientByID(id));
        }

        [HttpPost]
        public ActionResult EditCoefficient(int id, string vietnamese_coefficient, string foreign_coefficient, string theoretical_coefficient, string practice_coefficient)
        {
            // Update coefficient
            coefficient query_coefficient = unitOfWork.CoefficientRepository.GetCoefficientByID(id);
            query_coefficient.vietnamese_coefficient = decimal.Parse(vietnamese_coefficient, CultureInfo.InvariantCulture);
            query_coefficient.foreign_coefficient = decimal.Parse(foreign_coefficient, CultureInfo.InvariantCulture);
            query_coefficient.theoretical_coefficient = decimal.Parse(theoretical_coefficient, CultureInfo.InvariantCulture);
            query_coefficient.practice_coefficient = decimal.Parse(practice_coefficient, CultureInfo.InvariantCulture);
            unitOfWork.Save();
            return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Subject()
        {
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            ViewData["major"] = new SelectList(unitOfWork.MajorRepository.GetMajors(), "id", "name");
            return View();
        }

        [HttpGet]
        public ActionResult GetSubjectPartial(int termId, string majorId)
        {
            ViewData["termId"] = termId;
            ViewData["majorId"] = majorId;
            return PartialView("_Subject");
        }

        [HttpGet]
        public JsonResult GetSubjectData(int termId, string majorId)
        {
            // Get subjects data from database
            IEnumerable query_subjects = majorId != "-1"
                ? unitOfWork.SubjectRepository.GetSubjects(termId, majorId)
                : unitOfWork.SubjectRepository.GetTermSubjects(termId);
            return Json(query_subjects, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditSubject(string id)
        {
            return View(unitOfWork.SubjectRepository.GetSubjectByID(id));
        }

        [HttpPost]
        public ActionResult EditSubject(string id, bool is_vietnamese)
        {
            // Update subject
            subject query_subject = unitOfWork.SubjectRepository.GetSubjectByID(id);
            query_subject.is_vietnamese = is_vietnamese;
            unitOfWork.Save();
            return Json(new { success = true, message = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
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