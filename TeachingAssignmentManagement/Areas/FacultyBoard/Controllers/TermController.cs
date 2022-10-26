using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Areas.FacultyBoard.Controllers
{
    [Authorize(Roles = "BCN khoa")]
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
            // Get terms data from datatabse
            return Json(termRepository.GetTerms(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new term());
        }

        [HttpPost]
        public ActionResult Create(term term)
        {
            try
            {
                // Create new major
                termRepository.InsertTerm(term);
                termRepository.Save();
                return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { error = true }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            return View(termRepository.GetTermByID(id));
        }

        [HttpPost]
        public ActionResult Edit(term term)
        {
            // Update major
            termRepository.UpdateTerm(term);
            termRepository.Save();
            return Json(new { success = true, message = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
        }
    }
}