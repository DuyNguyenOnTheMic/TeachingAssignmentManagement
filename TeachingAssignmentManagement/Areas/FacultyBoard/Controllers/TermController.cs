using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Areas.FacultyBoard.Controllers
{
    public class TermController : Controller
    {
        private readonly ITermRepository termRepository;

        public TermController()
        {
            this.termRepository = new TermRepository(new CP25Team03Entities());
        }

        public TermController(ITermRepository termRepository)
        {
            this.termRepository = termRepository;
        }

        // GET: FacultyBoard/Term
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetData()
        {
            // Get majors data from datatabse
            return Json(termRepository.GetTerms(), JsonRequestBehavior.AllowGet);
        }

    }
}