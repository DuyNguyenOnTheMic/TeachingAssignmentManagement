using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace TeachingAssignmentManagement.Hubs
{
    public class CurriculumClassHub : Hub
    {
        [HubMethodName("broadcastDelete")]
        public static void BroadcastData(int id, bool isUpdate)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<CurriculumClassHub>();
            context.Clients.All.updatedData(id, isUpdate);
        }
    }
}