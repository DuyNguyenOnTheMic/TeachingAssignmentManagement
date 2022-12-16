using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Hubs;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Controllers
{
    [Authorize(Roles = "BCN khoa")]
    public class UserController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly UnitOfWork unitOfWork;

        public UserController()
        {
            unitOfWork = new UnitOfWork(new CP25Team03Entities());
        }

        public UserController(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
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

        [HttpGet]
        [OutputCache(Duration = 600, VaryByCustom = "userName")]
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetData()
        {
            // Get user data from database
            return Json(unitOfWork.UserRepository.GetUsers(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Import(string[] lecturerId, string[] lecturerName)
        {
            for (int i = 0; i < lecturerId.Length; i++)
            {
                string id = lecturerId[i].Trim();
                string fullName = lecturerName[i].Trim();
                try
                {
                    ApplicationUser user = new ApplicationUser
                    {
                        UserName = id
                    };
                    // Create new user
                    UserManager.Create(user);
                    UserManager.AddToRole(user.Id, "Giảng viên");

                    // Add to lecturer table
                    lecturer lecturer = new lecturer
                    {
                        id = user.Id,
                        staff_id = id,
                        full_name = fullName
                    };
                    unitOfWork.UserRepository.InsertLecturer(lecturer);
                    unitOfWork.Save();
                    ProgressHub.SendProgress("Đang import...", i, lecturerId.Length);
                }
                catch
                {
                    return Json(new { error = true, message = "Có lỗi đã xảy ra ở giảng viên <strong>" + id + " - " + fullName + "</strong>" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = true, message = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [OutputCache(Duration = 600, VaryByParam = "none")]
        public ActionResult Create()
        {
            ViewData["role_id"] = new SelectList(unitOfWork.UserRepository.GetRoles(), "id", "name");
            return View(new lecturer());
        }

        [HttpPost]
        public ActionResult Create(string staff_id, string full_name, string email, string type, string role_id)
        {
            // Declare variables
            string txtStaffId = SetNullOnEmpty(staff_id);
            string txtFullName = SetNullOnEmpty(full_name);
            AspNetRole role = unitOfWork.UserRepository.GetRoleByID(role_id);

            // Get user information
            ApplicationUser user = new ApplicationUser
            {
                Email = email,
                UserName = email
            };

            // Check if user exists
            ApplicationUser currentUser = UserManager.FindByEmail(user.Email);
            if (currentUser != null)
            {
                return Json(new { error = true, message = "Người dùng đã có trong hệ thống!" }, JsonRequestBehavior.AllowGet);
            }
            else if (txtStaffId != null || txtFullName != null)
            {
                try
                {
                    // Add a new lecturer
                    lecturer lecturer = new lecturer
                    {
                        id = user.Id,
                        staff_id = txtStaffId,
                        full_name = txtFullName,
                        type = type
                    };
                    unitOfWork.UserRepository.InsertLecturer(lecturer);
                    unitOfWork.Save();
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
            ApplicationUser user = UserManager.FindById(id);
            if (user.Roles.Count > 0)
            {
                // Set selected role
                ViewData["role_id"] = new SelectList(unitOfWork.UserRepository.GetRoles(), "id", "name", user.Roles.FirstOrDefault().RoleId);
            }
            else
            {
                // Populate new role select list
                ViewData["role_id"] = new SelectList(unitOfWork.UserRepository.GetRoles(), "id", "name");
            }
            ViewData["email"] = user.Email;
            return View(unitOfWork.UserRepository.GetLecturerByID(id));
        }

        [HttpPost]
        public ActionResult Edit(string id, string staff_id, string full_name, string email, string role_id)
        {
            // Declare variables
            ApplicationUser user = UserManager.FindById(id);
            ApplicationUser newUser = UserManager.FindByEmail(email);
            string txtStaffId = SetNullOnEmpty(staff_id);
            string txtFullName = SetNullOnEmpty(full_name);
            string oldRole = UserManager.GetRoles(id).FirstOrDefault();
            AspNetRole role = unitOfWork.UserRepository.GetRoleByID(role_id);
            lecturer query_lecturer = unitOfWork.UserRepository.GetLecturerByID(id);

            // Check if user exists in the system
            if (newUser != null && email != user.Email)
            {
                return Json(new { error = true, message = "Người dùng đã có trong hệ thống!" }, JsonRequestBehavior.AllowGet);
            }

            // Prevent user from editing the last faculty board role
            int facultyBoardCount = unitOfWork.UserRepository.GetFacultyBoards().Count();
            if (facultyBoardCount <= 1 && oldRole == "BCN khoa" && role.Name != "BCN khoa")
            {
                return Json(new { error = true, message = "Bạn không thể sửa BCN khoa cuối cùng!" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                if (query_lecturer != null)
                {
                    // Edit lecturer info
                    query_lecturer.staff_id = txtStaffId;
                    query_lecturer.full_name = txtFullName;
                }
                else if (txtStaffId != null || txtFullName != null)
                {
                    // Add a new lecturer
                    lecturer lecturer = new lecturer
                    {
                        id = id,
                        staff_id = txtStaffId,
                        full_name = txtFullName
                    };
                    unitOfWork.UserRepository.InsertLecturer(lecturer);
                }
                unitOfWork.Save();

                // Update email of user
                user.Email = email;
                user.UserName = email;
                UserManager.Update(user);
            }
            catch
            {
                return Json(new { error = true, message = "Mã giảng viên này đã có trong hệ thống!" }, JsonRequestBehavior.AllowGet);
            }

            if (oldRole == null)
            {
                // Add user to role
                UserManager.AddToRole(id, role.Name);
            }
            else
            {
                // Update user role
                UserManager.RemoveFromRole(id, oldRole);
                UserManager.AddToRole(id, role.Name);
            }
            return Json(new { success = true, message = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            // Declare variables
            ApplicationUser user = UserManager.FindById(id);
            string role = UserManager.GetRoles(id).FirstOrDefault();

            // Prevent user from deleting the last faculty board role
            int facultyBoardCount = unitOfWork.UserRepository.GetFacultyBoards().Count();
            if (facultyBoardCount <= 1 && role == "BCN khoa")
            {
                return Json(new { error = true, message = "Bạn không thể xoá BCN khoa cuối cùng!" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                // Delete user
                UserManager.Delete(user);
            }
            catch
            {
                return Json(new { error = true, message = "Không thể xoá do giảng viên này đã được phân công trong hệ thống!" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = true, message = "Xoá thành công!" }, JsonRequestBehavior.AllowGet);
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
            }

            unitOfWork.Dispose();
            base.Dispose(disposing);
        }

        #region Helpers
        public static string SetNullOnEmpty(string value)
        {
            // Check if string is empty
            return value != null && string.IsNullOrEmpty(value.Trim()) ? null : value;
        }
        #endregion
    }
}