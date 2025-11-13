using Microsoft.AspNetCore.SignalR;

namespace LabManagement.API.Hubs
{
    /// <summary>
    /// SignalR hub for real-time equipment status notifications
    /// </summary>
    public class EquipmentHub : Hub
    {
        private readonly ILogger<EquipmentHub> _logger;

        public EquipmentHub(ILogger<EquipmentHub> logger)
        {
            _logger = logger;
        }

        // Group for all managers
        public static string GetAllManagersGroupName() => "all-managers";
        
        // Group for specific lab managers
        public static string GetLabManagerGroupName(int labId) => $"lab-manager:{labId}";

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"Equipment Hub - Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _logger.LogInformation($"Equipment Hub - Client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }
        
        /// <summary>
        /// Join the all-managers group to receive all equipment notifications
        /// </summary>
        public async Task JoinAllManagersGroup()
        {
            try
            {
                _logger.LogInformation($"Client {Context.ConnectionId} joining all-managers group");
                await Groups.AddToGroupAsync(Context.ConnectionId, GetAllManagersGroupName());
                _logger.LogInformation($"Client {Context.ConnectionId} joined all-managers group successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error joining all-managers group for {Context.ConnectionId}");
                throw new HubException($"Failed to join all-managers group: {ex.Message}");
            }
        }

        /// <summary>
        /// Leave the all-managers group
        /// </summary>
        public async Task LeaveAllManagersGroup()
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetAllManagersGroupName());
                _logger.LogInformation($"Client {Context.ConnectionId} left all-managers group");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error leaving all-managers group for {Context.ConnectionId}");
                throw new HubException($"Failed to leave all-managers group: {ex.Message}");
            }
        }

        /// <summary>
        /// Join a specific lab's equipment notification group
        /// </summary>
        public async Task JoinLabGroup(int labId)
        {
            try
            {
                if (labId <= 0)
                    throw new HubException("Invalid lab ID");

                _logger.LogInformation($"Client {Context.ConnectionId} joining lab-{labId} group");
                await Groups.AddToGroupAsync(Context.ConnectionId, GetLabManagerGroupName(labId));
                _logger.LogInformation($"Client {Context.ConnectionId} joined lab-{labId} group successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error joining lab group {labId} for {Context.ConnectionId}");
                throw new HubException($"Failed to join lab group: {ex.Message}");
            }
        }

        /// <summary>
        /// Leave a specific lab's equipment notification group
        /// </summary>
        public async Task LeaveLabGroup(int labId)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetLabManagerGroupName(labId));
                _logger.LogInformation($"Client {Context.ConnectionId} left lab-{labId} group");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error leaving lab group {labId} for {Context.ConnectionId}");
                throw new HubException($"Failed to leave lab group: {ex.Message}");
            }
        }
    }
}
