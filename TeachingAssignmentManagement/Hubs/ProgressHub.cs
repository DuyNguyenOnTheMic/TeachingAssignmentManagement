using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace TeachingAssignmentManagement.Hubs
{
    public class ProgressHub : Hub
    {
        [HubMethodName("sendProgress")]
        public static void SendProgress(string progressMessage, int progressCount, int totalItems)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();
            int percentage = (progressCount * 100) / totalItems;
            context.Clients.All.addProgress(progressMessage, percentage + "%");
        }
    }
}