namespace LabManagement.BLL.DTOs;

public class CreateReportDTO
{
    public int? ZoneId { get; set; }
    public int? LabId { get; set; }
    public string ReportType { get; set; } = null!;
    public string? Content { get; set; }
    public int? UserId { get; set; }
    public string? PhotoUrl { get; set; }
}
