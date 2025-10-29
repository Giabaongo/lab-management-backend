namespace LabManagement.BLL.DTOs;

public class ReportDTO
{
    public int ReportId { get; set; }
    public int? ZoneId { get; set; }
    public int? LabId { get; set; }
    public string ReportType { get; set; } = null!;
    public string? Content { get; set; }
    public DateTime GeneratedAt { get; set; }
    public int? UserId { get; set; }
    public string? PhotoUrl { get; set; }
    
    // Navigation properties
    public string? LabName { get; set; }
    public string? ZoneName { get; set; }
}
