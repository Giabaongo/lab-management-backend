using System;
using System.Collections.Generic;

namespace LabManagement.DAL.Models;

public partial class SecurityLog
{
    public int LogId { get; set; }

    public int EventId { get; set; }

    public int SecurityId { get; set; }

    public int ActionType { get; set; }

    public DateTime LoggedAt { get; set; }

    public string? PhotoUrl { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    public string? Notes { get; set; }

    public virtual LabEvent Event { get; set; } = null!;

    public virtual User Security { get; set; } = null!;
}
