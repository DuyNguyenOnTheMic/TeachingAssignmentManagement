using Microsoft.AspNet.SignalR;

namespace TeachingAssignmentManagement.Hubs
{
    public class ProgressHub : Hub
    {
        public static void SendProgress(string progressMessage, int progressCount, int totalItems)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();
            int percentage = (progressCount * 100) / totalItems;
            context.Clients.All.addProgress(progressMessage, percentage + "%");
        }
    }
}