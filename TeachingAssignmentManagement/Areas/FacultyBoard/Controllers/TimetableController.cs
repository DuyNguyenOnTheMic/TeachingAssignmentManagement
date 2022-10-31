using System.Web.Mvc;

namespace TeachingAssignmentManagement.Areas.FacultyBoard.Controllers
{
    [Authorize(Roles = "BCN khoa")]
    public class TimetableController : Controller
    {
        // GET: FacultyBoard/Timetable
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Import()
        {
            return View();
        }
    }
}