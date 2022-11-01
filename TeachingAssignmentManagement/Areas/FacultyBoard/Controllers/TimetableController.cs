using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Areas.FacultyBoard.Controllers
{
    [Authorize(Roles = "BCN khoa")]
    public class TimetableController : Controller
    {
        private readonly ITermRepository termRepository;

        public TimetableController()
        {
            this.termRepository = new TermRepository(new CP25Team03Entities());
        }

        public TimetableController(ITermRepository termRepository)
        {
            this.termRepository = termRepository;
        }

        // GET: FacultyBoard/Timetable
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Import()
        {
            ViewBag.term = termRepository.GetTerms();
            return View();
        }
    }
}