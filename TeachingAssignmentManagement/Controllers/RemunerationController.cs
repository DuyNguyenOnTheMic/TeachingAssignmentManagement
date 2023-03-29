using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Helpers;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Controllers
{
    [Authorize(Roles = CustomRoles.FacultyBoardOrDepartment)]
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
        public ActionResult GetData(int termId)
        {
            // Declare variables
            term term = unitOfWork.TermRepository.GetTermByID(termId);
            int startYear = term.start_year;
            int endYear = term.end_year;
            coefficient coefficient = unitOfWork.CoefficientRepository.GetCoefficientInYear(startYear, endYear);

            // Check if coefficient is null
            if (coefficient == null)
            {
                return PartialView("_Error");
            }

            // Keep declaring variables
            IEnumerable<LecturerRankDTO> lecturerRanks = unitOfWork.LecturerRankRepository.GetLecturerRanksInTerm(termId);
            IEnumerable<unit_price> unitPrice = unitOfWork.UnitPriceRepository.GetUnitPriceInYear(startYear, endYear);
            List<RemunerationDTO> remunerationDTOs = new List<RemunerationDTO>();

            foreach (LecturerRankDTO rank in lecturerRanks)
            {
                // Reset values in each loop
                decimal unitPriceByLevel = decimal.Zero,
                        teachingRemuneration = decimal.Zero;
                bool isMissing = false;

                // Check if lecturer have been assigned a rank
                if (rank.Id != null)
                {
                    // Get classes in term of lecturer
                    IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetPersonalClassesInTerm(termId, rank.LecturerId);
                    foreach (class_section item in query_classes)
                    {
                        // Get unit price for lecturer rank
                        int unitPriceType = rank.IsVietnamese == false ? MyConstants.ForeignType : item.major.program_type;
                        unit_price query_unitPrice = unitPrice.SingleOrDefault(u => u.academic_degree_rank_id == rank.AcademicDegreeRankId && u.type == unitPriceType);
                        if (query_unitPrice != null)
                        {
                            unitPriceByLevel = query_unitPrice.unit_price1;
                        }
                        else
                        {
                            isMissing = true;
                        }

                        teachingRemuneration += unitPriceByLevel * CalculateRemuneration(item, coefficient) * (decimal)item.total_lesson;
                    }
                }
                string lecturerRank = rank.AcademicDegreeRankId;
                string academicDegreeRankId = lecturerRank != null ? $"{lecturerRank} ({unitPriceByLevel:N0} ₫)" : null;
                remunerationDTOs.Add(new RemunerationDTO
                {
                    StaffId = rank.StaffId,
                    FullName = rank.FullName,
                    AcademicDegreeRankId = academicDegreeRankId,
                    Remuneration = teachingRemuneration,
                    Status = isMissing
                });
            }
            return PartialView("_Remuneration", remunerationDTOs);
        }

        public static decimal CalculateRemuneration(class_section classSection, coefficient coefficient)
        {
            decimal crowdedClassCoefficient, timeCoefficient, languageCoefficient, classTypeCoefficient;

            // Check if class is theoretical or practice
            int studentNumber;
            if (classSection.type == MyConstants.TheoreticalClassType)
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
            return crowdedClassCoefficient * timeCoefficient * classTypeCoefficient * languageCoefficient;
        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}