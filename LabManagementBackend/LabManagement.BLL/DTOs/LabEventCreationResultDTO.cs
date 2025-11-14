namespace LabManagement.BLL.DTOs
{
    public class LabEventCreationResultDTO
    {
        public LabEventDTO Event { get; set; } = null!;
        public List<CancelledItemDTO> CancelledBookings { get; set; } = new();
        public List<CancelledItemDTO> CancelledEvents { get; set; } = new();
        public int TotalCancelled => CancelledBookings.Count + CancelledEvents.Count;
    }

    public class CancelledItemDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Notes { get; set; }
    }
}
