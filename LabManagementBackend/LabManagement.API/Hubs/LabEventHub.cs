using Microsoft.AspNetCore.SignalR;

namespace LabManagement.API.Hubs
{
    /// <summary>
    /// SignalR hub for real-time lab event notifications
    /// </summary>
    public class LabEventHub : Hub
    {
        // Group for all users interested in lab events
        public static string GetAllEventsGroupName() => "all-events";
        
        // Group for specific lab events
        public static string GetLabEventsGroupName(int labId) => $"lab-events:{labId}";
        
        // Group for users subscribed to a specific event
        public static string GetEventSubscribersGroupName(int eventId) => $"event:{eventId}";
        
        /// <summary>
        /// Join all events group
        /// </summary>
        public Task JoinAllEventsGroup()
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, GetAllEventsGroupName());
        }

        /// <summary>
        /// Leave all events group
        /// </summary>
        public Task LeaveAllEventsGroup()
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, GetAllEventsGroupName());
        }

        /// <summary>
        /// Subscribe to a specific lab's events
        /// </summary>
        public Task SubscribeToLabEvents(int labId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, GetLabEventsGroupName(labId));
        }

        /// <summary>
        /// Unsubscribe from a specific lab's events
        /// </summary>
        public Task UnsubscribeFromLabEvents(int labId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, GetLabEventsGroupName(labId));
        }

        /// <summary>
        /// Subscribe to a specific event for updates
        /// </summary>
        public Task SubscribeToEvent(int eventId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, GetEventSubscribersGroupName(eventId));
        }

        /// <summary>
        /// Unsubscribe from a specific event
        /// </summary>
        public Task UnsubscribeFromEvent(int eventId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, GetEventSubscribersGroupName(eventId));
        }
    }
}
