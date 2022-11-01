using System.Web;
using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Areas.FacultyBoard.Controllers
{
    [Authorize(Roles = "BCN khoa")]
    public class TimetableController : Controller
    {
        private readonly ITermRepository termRepository;
        private readonly IMajorRepository majorRepository;

        public TimetableController()
        {
            this.termRepository = new TermRepository(new CP25Team03Entities());
            this.majorRepository = new MajorRepository(new CP25Team03Entities());
        }

        public TimetableController(ITermRepository termRepository, IMajorRepository majorRepository)
        {
            this.termRepository = termRepository;
            this.majorRepository = majorRepository;
        }

        // GET: FacultyBoard/Timetable
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Import()
        {
            ViewBag.term = new SelectList(termRepository.GetTerms(), "id", "id");
            ViewBag.major = new SelectList(majorRepository.GetMajors(), "id", "name");
            return View();
        }

        [HttpPost]
        public ActionResult Import(HttpPostedFileBase postedFile)
        {
            return RedirectToAction("Import");
        }
    }
}