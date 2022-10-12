using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Areas.FacultyBoard.Controllers
{
    public class UserController : Controller
    {
        readonly CP25Team03Entities db = new CP25Team03Entities();
        private ApplicationUserManager _userManager;

        public UserController()
        {
        }

        public UserController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Admin/Role
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetData()
        {
            // Get user data from datatabse
            return Json(db.AspNetUsers.Select(u => new
            {
                id = u.Id,
                email = u.Email,
                role = u.AspNetRoles.FirstOrDefault().Name,

            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            // Get user role
            var query_role = db.AspNetUsers.Find(id).AspNetRoles.FirstOrDefault();
            if (query_role != null)
            {
                // Set selected role
                ViewBag.role_id = new SelectList(db.AspNetRoles, "id", "name", query_role.Id);
            }
            else
            {
                // Populate new role select list
                ViewBag.role_id = new SelectList(db.AspNetRoles, "id", "name");
            }

            return View(db.AspNetUsers.Find(id));
        }

        [HttpPost]
        public ActionResult Edit(AspNetUser aspNetUser, string role_id)
        {
            // Declare variables
            var oldUser = UserManager.FindById(aspNetUser.Id);
            var oldRole = UserManager.GetRoles(oldUser.Id).FirstOrDefault();
            var role = db.AspNetRoles.Find(role_id);
            var result = new IdentityResult();

            // Prevent user from editing the last admin role
            int adminCount = db.AspNetUsers.Where(u => u.AspNetRoles.FirstOrDefault().Name == "Admin").Count();
            if (adminCount <= 1 && oldRole == "Admin" && role.Name != "Admin")
            {
                return Json(new { result.Errors }, JsonRequestBehavior.AllowGet);
            }

            if (oldRole == null)
            {
                // Add user to role
                result = UserManager.AddToRole(aspNetUser.Id, role.Name);
            }
            else
            {
                // Update user role
                UserManager.RemoveFromRole(aspNetUser.Id, oldRole);
                result = UserManager.AddToRole(aspNetUser.Id, role.Name);
            }

            return Json(new { result.Succeeded, message = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            // Declare variables
            var user = UserManager.FindById(id);
            var role = UserManager.GetRoles(id).FirstOrDefault();
            var result = new IdentityResult();

            // Prevent user from deleting the last admin role
            int adminCount = db.AspNetUsers.Where(u => u.AspNetRoles.FirstOrDefault().Name == "Admin").Count();
            if (adminCount <= 1 && role == "Admin")
            {
                return Json(new { result.Errors }, JsonRequestBehavior.AllowGet);
            }

            // Delete user
            result = UserManager.Delete(user);
            return Json(new { result.Succeeded, message = "Xoá thành công!" }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}