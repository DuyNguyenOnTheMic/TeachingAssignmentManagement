using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager)
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
            // Get user information
            var user = new ApplicationUser
            {
                Email = User.Identity.Name,
                UserName = User.Identity.Name
            };

            // Check if user exists
            var currentUser = await UserManager.FindByEmailAsync(user.Email);
            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            if (currentUser != null)
            {
                if (currentUser.Roles.Count != 0)
                {
                    // Add role claim to user
                    var currentRole = await UserManager.GetRolesAsync(currentUser.Id);
                    identity.AddClaim(new Claim(ClaimTypes.Role, currentRole[0]));
                }
            }
            else
            {
                // Create new user
                string newUserRole = "Chưa phân quyền";
                await UserManager.CreateAsync(user);
                await UserManager.AddToRoleAsync(user.Id, newUserRole);
                identity.AddClaim(new Claim(ClaimTypes.Role, newUserRole));
            }
            IOwinContext context = HttpContext.GetOwinContext();
            context.Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            context.Authentication.SignIn(identity);

            return RedirectToAction("Index", "Home");
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

            base.Dispose(disposing);
        }
    }
}