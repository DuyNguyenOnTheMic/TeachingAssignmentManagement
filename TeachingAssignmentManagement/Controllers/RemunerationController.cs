using System.Web.Mvc;

namespace TeachingAssignmentManagement.Controllers
{
    public class RemunerationController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Subject()
        {
            return View();
        }
    }
}