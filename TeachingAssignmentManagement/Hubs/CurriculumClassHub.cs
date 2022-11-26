using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace TeachingAssignmentManagement.Hubs
{
    public class CurriculumClassHub : Hub
    {
        [HubMethodName("broadcastData")]
        public static void BroadcastData(int id, string lecturerId, string lecturerName, bool isUpdate)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<CurriculumClassHub>();
            context.Clients.All.updatedData(id, lecturerId, lecturerName, isUpdate);
        }

        [HubMethodName("refreshData")]
        public static void RefreshData()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<CurriculumClassHub>();
            context.Clients.All.refreshedData();
        }
    }
}