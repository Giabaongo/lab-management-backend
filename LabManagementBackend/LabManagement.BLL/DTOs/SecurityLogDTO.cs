namespace LabManagement.BLL.DTOs
{
    public class SecurityLogDTO
    {
        public int LogId { get; set; }
        public int EventId { get; set; }
        public int SecurityId { get; set; }
        public int Action { get; set; }
        public DateTime Timestamp { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Notes { get; set; }
    }
}

