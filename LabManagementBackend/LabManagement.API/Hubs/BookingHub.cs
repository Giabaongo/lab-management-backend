using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace LabManagement.API.Hubs
{
    /// <summary>
    /// SignalR hub to broadcast booking events to lab managers in real time.
    /// </summary>
    public class BookingHub : Hub
    {
        public static string GetManagerGroupName(int managerId) => $"manager:{managerId}";

        public Task JoinManagerGroup(int managerId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, GetManagerGroupName(managerId));
        }

        public Task LeaveManagerGroup(int managerId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, GetManagerGroupName(managerId));
        }
    }
}
