using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace TeachingAssignmentManagement.Hubs
{
    public class CurriculumClassHub : Hub
    {
        [HubMethodName("broadcastData")]
        public static void BroadcastData()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<CurriculumClassHub>();
            context.Clients.All.updatedData();
        }
    }
}