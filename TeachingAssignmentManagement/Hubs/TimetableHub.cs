using Microsoft.AspNet.SignalR;

namespace TeachingAssignmentManagement.Hubs
{
    public class TimetableHub : Hub
    {
        public static void BroadcastData(int id, string lecturerId, string lecturerName, bool isUpdate)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<TimetableHub>();
            context.Clients.All.updatedData(id, lecturerId, lecturerName, isUpdate);
        }

        public static void RefreshData(int term, string major)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<TimetableHub>();
            context.Clients.All.refreshedData(term, major);
        }
    }
}