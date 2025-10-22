namespace LabManagement.BLL.DTOs
{
    public class LabEventDTO
    {
        public int EventId { get; set; }
        public int LabId { get; set; }
        public int ZoneId { get; set; }
        public int ActivityTypeId { get; set; }
        public int OrganizerId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
