using System.Collections.Generic;
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
                decimal teachingRemuneration = decimal.Zero;
                bool isMissing = false;

                // Check if lecturer have been assigned a rank
                if (rank.Id != null)
                {
                    decimal unitPriceByLevel;

                    // Get classes in term of lecturer
                    IEnumerable<class_section> query_classes = unitOfWork.ClassSectionRepository.GetPersonalClassesInTerm(termId, rank.LecturerId);
                    foreach (class_section item in query_classes)
                    {
                        // Get unit price for lecturer rank
                        int unitPriceType = rank.IsVietnamese == false ? Constants.ForeignType : item.major.program_type;
                        unit_price query_unitPrice = unitPrice.SingleOrDefault(u => u.academic_degree_rank_id == rank.AcademicDegreeRankId && u.type == unitPriceType);
                        if (query_unitPrice != null)
                        {
                            unitPriceByLevel = query_unitPrice.unit_price1;
                        }
                        else
                        {
                            isMissing = true;
                            unitPriceByLevel = decimal.Zero;
                        }

                        teachingRemuneration += unitPriceByLevel * CalculateRemuneration(item, coefficient);
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
            return PartialView("_Remuneration", remunerationDTOs);
        }

        public static decimal CalculateRemuneration(class_section classSection, coefficient coefficient)
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
            return crowdedClassCoefficient * timeCoefficient * classTypeCoefficient * languageCoefficient;
        }

        protected override void Dispose(bool disposing)
        {
            unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}