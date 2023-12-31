﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TeachingAssignmentManagement.DAL;
using TeachingAssignmentManagement.Helpers;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationUserManager _userManager;
        private readonly UnitOfWork unitOfWork = new UnitOfWork(new CP25Team03Entities());

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

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (!Request.IsAuthenticated)
            {
                ViewBag.ReturnUrl = "/";
                return View();
            }
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/LoginRedirect
        [AllowAnonymous]
        public ActionResult LoginRedirect()
        {
            return RedirectToAction("Login");
        }

        //
        // POST: /Account/SignIn
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public void SignIn()
        {
            // Send an OpenID Connect sign-in request.
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = Url.Action("SignInCallBack") }, OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }

        //
        // GET: /Account/SignInCallBack
        public async Task<ActionResult> SignInCallBack()
        {
            // Declare variables
            IOwinContext context = HttpContext.GetOwinContext();
            string userEmail = User.Identity.Name;
            ApplicationUser user = new ApplicationUser
            {
                Email = userEmail,
                UserName = userEmail
            };

            // Check if user exists
            ApplicationUser currentUser = await UserManager.FindByEmailAsync(userEmail);
            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            Claim existingClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
            identity.RemoveClaim(existingClaim);
            if (currentUser != null)
            {
                // Check if user status is available
                if (GetStatus(currentUser.Id) == false)
                {
                    context.Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
                    return View("Lockout");
                }
                else if (currentUser.Roles.Count != 0)
                {
                    // Add role claim to user
                    System.Collections.Generic.IList<string> currentRole = await UserManager.GetRolesAsync(currentUser.Id);
                    identity.AddClaim(new Claim(ClaimTypes.Role, currentRole[0]));
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, currentUser.Id));
                }
            }
            else
            {
                // Create new user
                string newUserRole = "Chưa phân quyền";
                await UserManager.CreateAsync(user);
                await UserManager.AddToRoleAsync(user.Id, newUserRole);
                identity.AddClaim(new Claim(ClaimTypes.Role, newUserRole));
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            }
            context.Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            context.Authentication.SignIn(identity);

            // Redirect user to specific page based on role
            string role = identity.GetRole();
            switch (role)
            {
                case CustomRoles.Department:
                case CustomRoles.Lecturer:
                    return RedirectToAction("Index", "Timetable");
                default:
                    return RedirectToAction("Index", "Home");
            }
        }

        public bool GetStatus(string userId)
        {
            // Get lecturer status
            lecturer lecturer = unitOfWork.UserRepository.GetLecturerByID(userId);
            return lecturer == null || (bool)lecturer.status;
        }

        //
        // POST: /Account/SignOut
        [HttpPost]
        public ActionResult SignOut()
        {
            /// Send an OpenID Connect sign-out request.
            HttpContext.GetOwinContext()
                        .Authentication
                        .SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Redirect("Login");
        }

        //
        // GET: /Account/Update
        public ActionResult Update()
        {
            // Return update user profile view
            string userId = User.Identity.GetUserId();
            return View(unitOfWork.UserRepository.GetLecturerByID(userId));
        }

        //
        // POST: /Account/Update
        [HttpPost]
        public ActionResult Update([Bind(Include = "staff_id,full_name")] lecturer lecturer)
        {
            string userId = User.Identity.GetUserId();
            try
            {
                lecturer query_lecturer = unitOfWork.UserRepository.GetLecturerByID(userId);
                if (query_lecturer != null)
                {
                    // Edit lecturer info
                    query_lecturer.staff_id = lecturer.staff_id;
                    query_lecturer.full_name = lecturer.full_name;
                }
                else
                {
                    // Create a new lecturer
                    lecturer.id = userId;
                    lecturer.status = true;
                    unitOfWork.UserRepository.InsertLecturer(lecturer);
                }
                unitOfWork.Save();
                return Json(new { success = true, message = "Cập nhật thành công!" }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { error = true, message = "Mã giảng viên này đã có trong hệ thống!" }, JsonRequestBehavior.AllowGet);
            }
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
    }
}