using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TeachingAssignmentManagement.Startup))]
namespace TeachingAssignmentManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
