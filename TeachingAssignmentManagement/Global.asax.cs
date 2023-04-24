using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace TeachingAssignmentManagement
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalFilters.Filters.Add(new CompressAttribute());
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        public override string GetVaryByCustomString(HttpContext context, string arg)
        {
            if (arg == "userName")
            {
                return context.User.Identity.Name;
            }
            return string.Empty;
        }
    }
}
