using System;
using System.Collections.Generic;

namespace LabManagement.DAL.Models;

public partial class EventParticipant
{
    public int EventId { get; set; }

    public int UserId { get; set; }

    public string Role { get; set; } = null!;

    public virtual LabEvent Event { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
