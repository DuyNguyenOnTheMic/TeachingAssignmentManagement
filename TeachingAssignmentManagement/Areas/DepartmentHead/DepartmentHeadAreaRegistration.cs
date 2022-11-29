using System.Web.Mvc;

namespace TeachingAssignmentManagement.Areas.DepartmentHead
{
    public class DepartmentHeadAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "DepartmentHead";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "DepartmentHead_default",
                "DepartmentHead/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}