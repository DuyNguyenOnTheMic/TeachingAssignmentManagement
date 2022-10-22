using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Areas.FacultyBoard.Controllers
{
    [Authorize(Roles = "BCN khoa")]
    public class UserController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly IUserRepository userRepository;

        public UserController()
        {
            this.userRepository = new UserRepository(new CP25Team03Entities());
        }

        public UserController(ApplicationUserManager userManager, IUserRepository userRepository)
        {
            UserManager = userManager;
            this.userRepository = userRepository;
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

        // GET: FacultyBoard/User
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetData()
        {
            // Get user data from datatabse
            return Json(userRepository.GetUsers(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.role_id = new SelectList(userRepository.GetRoles(), "id", "name");
            return View(new lecturer());
        }

        [HttpPost]
        public ActionResult Create(string staff_id, string full_name, string email, string role_id)
        {
            // Declare variables
            string txtStaffId = SetNullOnEmpty(staff_id);
            string txtFullName = SetNullOnEmpty(full_name);
            var role = userRepository.GetRoleByID(role_id);

            // Get user information
            var user = new ApplicationUser
            {
                Email = email,
                UserName = email
            };

            // Check if user exists
            var currentUser = UserManager.FindByEmail(user.Email);
            if (currentUser != null)
            {
                return Json(new { error = true, message = "Người dùng đã có trong hệ thống!" }, JsonRequestBehavior.AllowGet);
            }
            else if (txtStaffId != null || txtFullName != null)
            {
                try
                {
                    // Add a new lecturer
                    var lecturer = new lecturer
                    {
                        id = user.Id,
                        staff_id = txtStaffId,
                        full_name = txtFullName
                    };
                    userRepository.InsertLecturer(lecturer);
                    userRepository.Save();
                }
                catch
                {
                    return Json(new { error = true, message = "Mã giảng viên này đã có trong hệ thống!" }, JsonRequestBehavior.AllowGet);
                }
            }
            // Create new user
            UserManager.Create(user);
            UserManager.AddToRole(user.Id, role.Name);
            return Json(new { success = true, message = "Lưu thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Edit(string id)
        {
            // Get user role
            var query_user = UserManager.FindById(id);
            if (query_user.Roles.Count > 0)
            {
                // Set selected role
                ViewBag.role_id = new SelectList(userRepository.GetRoles(), "id", "name", query_user.Roles.FirstOrDefault().RoleId);
            }
            else
            {
                // Populate new role select list
                ViewBag.role_id = new SelectList(userRepository.GetRoles(), "id", "name");
            }
            ViewBag.email = query_user.Email;
            return View(userRepository.GetLecturerByID(id));
        }

        [HttpPost]
        public ActionResult Edit(string staff_id, string full_name, string email, string role_id)
        {
            // Declare variables
            string txtStaffId = SetNullOnEmpty(staff_id);
            string txtFullName = SetNullOnEmpty(full_name);
            var userId = UserManager.FindByEmail(email).Id;
            var oldRole = UserManager.GetRoles(userId).FirstOrDefault();
            var role = userRepository.GetRoleByID(role_id);
            var query_lecturer = userRepository.GetLecturerByID(userId);

            if (query_lecturer != null)
            {
                // Edit lecturer info
                query_lecturer.staff_id = txtStaffId;
                query_lecturer.full_name = txtFullName;
                userRepository.Save();
            }
            else if (txtStaffId != null || txtFullName != null)
            {
                // Add a new lecturer
                var lecturer = new lecturer
                {
                    id = userId,
                    staff_id = txtStaffId,
                    full_name = txtFullName
                };
                userRepository.InsertLecturer(lecturer);
                userRepository.Save();
            }

            // Prevent user from editing the last faculty board role
            int facultyBoardCount = userRepository.GetFacultyBoards().Count();
            if (facultyBoardCount <= 1 && oldRole == "BCN khoa" && role.Name != "BCN khoa")
            {
                return Json(new { error = true, message = "Bạn không thể sửa BCN khoa cuối cùng!" }, JsonRequestBehavior.AllowGet);
            }

            if (oldRole == null)
            {
                // Add user to role
                UserManager.AddToRole(userId, role.Name);
            }
            else
            {
                // Update user role
                UserManager.RemoveFromRole(userId, oldRole);
                UserManager.AddToRole(userId, role.Name);
            }
            return Json(new { success = true, message = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            // Declare variables
            var user = UserManager.FindById(id);
            var role = UserManager.GetRoles(id).FirstOrDefault();

            // Prevent user from deleting the last faculty board role
            int facultyBoardCount = userRepository.GetFacultyBoards().Count();
            if (facultyBoardCount <= 1 && role == "BCN khoa")
            {
                return Json(new { error = true, message = "Bạn không thể xoá BCN khoa cuối cùng!" }, JsonRequestBehavior.AllowGet);
            }

            // Delete user
            UserManager.Delete(user);
            return Json(new { success = true, message = "Xoá thành công!" }, JsonRequestBehavior.AllowGet);
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
                userRepository.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}