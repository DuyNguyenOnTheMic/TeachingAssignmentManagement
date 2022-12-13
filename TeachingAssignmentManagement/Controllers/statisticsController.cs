using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly UnitOfWork unitOfWork;

        public StatisticsController()
        {
            unitOfWork = new UnitOfWork(new CP25Team03Entities());
        }

        public StatisticsController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: Statistics
        public ActionResult Index()
        {
            ViewData["term"] = new SelectList(unitOfWork.TermRepository.GetTerms(), "id", "id");
            ViewData["year"] = new SelectList(unitOfWork.TermRepository.GetYear(), "schoolyear", "schoolyear");
            return View();
        }
    }
}