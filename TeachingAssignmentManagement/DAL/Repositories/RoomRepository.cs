using TeachingAssignmentManagement.Models;

namespace TeachingAssignmentManagement.DAL
{
    public class RoomRepository
    {
        private readonly CP25Team03Entities context;

        public RoomRepository(CP25Team03Entities context)
        {
            this.context = context;
        }

        public void InsertRoom(room room)
        {
            context.rooms.Add(room);
        }
    }
}