using System;
using System.Collections.Generic;

namespace LabManagement.DAL.Models;

public partial class Booking
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

    public virtual Lab Lab { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual LabZone Zone { get; set; } = null!;
}
