using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Areas.FacultyBoard.Controllers
{
    [Authorize(Roles = "BCN khoa")]
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
            var query_studyResult = db.lecturers;
            return Json(db.AspNetUsers.Select(u => new
            {
                id = u.Id,
                email = u.Email,
                role = u.AspNetRoles.FirstOrDefault().Name,
                query_studyResult.FirstOrDefault(l => l.email == u.Email).staff_id,
                query_studyResult.FirstOrDefault(l => l.email == u.Email).full_name
            }).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            // Get user role
            var query_user = UserManager.FindById(id);
            var query_lecturer = db.lecturers.FirstOrDefault(l => l.email == query_user.Email);
            if (query_user.Roles.Count > 0)
            {
                // Set selected role
                ViewBag.role_id = new SelectList(db.AspNetRoles, "id", "name", query_user.Roles.FirstOrDefault().RoleId);
            }
            else
            {
                // Populate new role select list
                ViewBag.role_id = new SelectList(db.AspNetRoles, "id", "name");
            }
            ViewBag.email = query_user.Email;
            return View(query_lecturer);
        }

        [HttpPost]
        public ActionResult Edit(string staff_id, string full_name, string email, string role_id)
        {
            // Declare variables
            string txtStaffId = SetNullOnEmpty(staff_id);
            string txtFullName = SetNullOnEmpty(full_name);
            var oldUser = UserManager.FindByEmail(email);
            var oldRole = UserManager.GetRoles(oldUser.Id).FirstOrDefault();
            var role = db.AspNetRoles.Find(role_id);
            var query_lecturer = db.lecturers.FirstOrDefault(l => l.email == email);
            var result = new IdentityResult();

            if (query_lecturer != null)
            {
                // Edit lecturer info
                query_lecturer.staff_id = txtStaffId;
                query_lecturer.full_name = txtFullName;
                db.SaveChanges();
            }
            else if (txtStaffId != null || txtFullName != null)
            {
                // Add a new lecturer
                var lecturer = new lecturer
                {
                    staff_id = txtStaffId,
                    full_name = txtFullName,
                    email = email
                };
                db.lecturers.Add(lecturer);
                db.SaveChanges();
            }

            // Prevent user from editing the last Faculty board role
            int adminCount = db.AspNetUsers.Where(u => u.AspNetRoles.FirstOrDefault().Name == "BCN khoa").Count();
            if (adminCount <= 1 && oldRole == "BCN khoa" && role.Name != "BCN khoa")
            {
                return Json(new { result.Errors, message = "Bạn không thể sửa BCN khoa cuối cùng!" }, JsonRequestBehavior.AllowGet);
            }

            if (oldRole == null)
            {
                // Add user to role
                result = UserManager.AddToRole(oldUser.Id, role.Name);
            }
            else
            {
                // Update user role
                UserManager.RemoveFromRole(oldUser.Id, oldRole);
                result = UserManager.AddToRole(oldUser.Id, role.Name);
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
            int adminCount = db.AspNetUsers.Where(u => u.AspNetRoles.FirstOrDefault().Name == "BCN khoa").Count();
            if (adminCount <= 1 && role == "BCN khoa")
            {
                return Json(new { result.Errors, message = "Bạn không thể xoá BCN khoa cuối cùng!" }, JsonRequestBehavior.AllowGet);
            }

            // Delete user
            result = UserManager.Delete(user);
            return Json(new { result.Succeeded, message = "Xoá thành công!" }, JsonRequestBehavior.AllowGet);
        }

        public static string SetNullOnEmpty(string value)
        {
            // Check if string is empty
            return value != null && string.IsNullOrEmpty(value.Trim()) ? null : value;
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