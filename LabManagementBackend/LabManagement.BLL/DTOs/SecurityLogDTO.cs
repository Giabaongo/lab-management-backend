namespace LabManagement.BLL.DTOs
{
    public class SecurityLogDTO
    {
        public int LogId { get; set; }
        public int EventId { get; set; }
        public int SecurityId { get; set; }
        public int ActionType { get; set; }
        public DateTime LoggedAt { get; set; }
        public string? PhotoUrl { get; set; }
        public string? Notes { get; set; }
    }
}

