using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace LabManagement.API.Hubs
{
    /// <summary>
    /// SignalR hub to broadcast booking events to lab managers in real time.
    /// </summary>
    public class BookingHub : Hub
    {
        private readonly ILogger<BookingHub> _logger;

        public BookingHub(ILogger<BookingHub> logger)
        {
            _logger = logger;
        }

        public static string GetManagerGroupName(int managerId) => $"manager:{managerId}";

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"Client disconnected: {Context.ConnectionId}. Exception: {exception?.Message}");
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Join a manager group to receive booking notifications for a specific manager.
        /// Frontend must send managerId as a number (int), NOT as a string.
        /// </summary>
        public async Task JoinManagerGroup(int managerId)
        {
            try
            {
                _logger.LogInformation($"Client {Context.ConnectionId} attempting to join manager group: {managerId}");
                
                if (managerId <= 0)
                {
                    _logger.LogWarning($"Invalid managerId: {managerId} from connection {Context.ConnectionId}");
                    throw new HubException("Invalid manager ID. Must be greater than 0.");
                }

                var groupName = GetManagerGroupName(managerId);
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                
                _logger.LogInformation($"Client {Context.ConnectionId} successfully joined group: {groupName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding client {Context.ConnectionId} to manager group {managerId}");
                throw new HubException($"Failed to join manager group: {ex.Message}");
            }
        }

        /// <summary>
        /// Leave a manager group to stop receiving booking notifications.
        /// Frontend must send managerId as a number (int), NOT as a string.
        /// </summary>
        public async Task LeaveManagerGroup(int managerId)
        {
            try
            {
                _logger.LogInformation($"Client {Context.ConnectionId} attempting to leave manager group: {managerId}");
                
                if (managerId <= 0)
                {
                    _logger.LogWarning($"Invalid managerId: {managerId} from connection {Context.ConnectionId}");
                    throw new HubException("Invalid manager ID. Must be greater than 0.");
                }

                var groupName = GetManagerGroupName(managerId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
                
                _logger.LogInformation($"Client {Context.ConnectionId} successfully left group: {groupName}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing client {Context.ConnectionId} from manager group {managerId}");
                throw new HubException($"Failed to leave manager group: {ex.Message}");
            }
        }
    }
}
