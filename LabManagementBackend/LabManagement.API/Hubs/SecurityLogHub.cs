using Microsoft.AspNetCore.SignalR;

namespace LabManagement.API.Hubs
{
    /// <summary>
    /// SignalR hub for real-time security log alerts
    /// </summary>
    public class SecurityLogHub : Hub
    {
        // Group for security team (Admin, SchoolManager)
        public static string GetSecurityTeamGroupName() => "security-team";
        
        // Group for specific lab managers
        public static string GetLabSecurityGroupName(int labId) => $"lab-security:{labId}";
        
        /// <summary>
        /// Join security team group to receive all security alerts
        /// </summary>
        public Task JoinSecurityTeamGroup()
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, GetSecurityTeamGroupName());
        }

        /// <summary>
        /// Leave security team group
        /// </summary>
        public Task LeaveSecurityTeamGroup()
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, GetSecurityTeamGroupName());
        }

        /// <summary>
        /// Join specific lab's security alerts
        /// </summary>
        public Task JoinLabSecurityGroup(int labId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, GetLabSecurityGroupName(labId));
        }

        /// <summary>
        /// Leave specific lab's security alerts
        /// </summary>
        public Task LeaveLabSecurityGroup(int labId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, GetLabSecurityGroupName(labId));
        }
    }
}
