using System;
using System.Collections.Generic;

namespace LabManagement.DAL.Models;

public partial class Report
{
    public int ReportId { get; set; }

    public int? ZoneId { get; set; }

    public int? LabId { get; set; }

    public string ReportType { get; set; } = null!;

    public string? Content { get; set; }

    public DateTime GeneratedAt { get; set; }

    public int? UserId { get; set; }

    public string? PhotoUrl { get; set; }

    public virtual Lab? Lab { get; set; }

    public virtual LabZone? Zone { get; set; }
}
