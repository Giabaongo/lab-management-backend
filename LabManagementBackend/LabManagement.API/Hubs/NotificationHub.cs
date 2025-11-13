using Microsoft.AspNetCore.SignalR;

namespace LabManagement.API.Hubs
{
    /// <summary>
    /// SignalR hub for general notification system
    /// </summary>
    public class NotificationHub : Hub
    {
        // Group for specific user notifications
        public static string GetUserGroupName(int userId) => $"user:{userId}";
        
        // Group for role-based notifications
        public static string GetRoleGroupName(string role) => $"role:{role}";
        
        /// <summary>
        /// Join user's personal notification group
        /// </summary>
        public Task JoinUserGroup(int userId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, GetUserGroupName(userId));
        }

        /// <summary>
        /// Leave user's personal notification group
        /// </summary>
        public Task LeaveUserGroup(int userId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, GetUserGroupName(userId));
        }

        /// <summary>
        /// Join role-based notification group
        /// </summary>
        public Task JoinRoleGroup(string role)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, GetRoleGroupName(role));
        }

        /// <summary>
        /// Leave role-based notification group
        /// </summary>
        public Task LeaveRoleGroup(string role)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, GetRoleGroupName(role));
        }
    }
}
