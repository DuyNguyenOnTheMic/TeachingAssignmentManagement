using System.Web.Mvc;

namespace TeachingAssignmentManagement.Areas.FacultyBoard
{
    public class FacultyBoardAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "FacultyBoard";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "FacultyBoard_default",
                "FacultyBoard/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}