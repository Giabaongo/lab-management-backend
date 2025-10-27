namespace LabManagement.BLL.DTOs
{
    public class BookingDTO
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int LabId { get; set; }
        public int ZoneId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Notes { get; set; }
    }
}
