using System;
using System.Collections.Generic;

namespace LabManagement.DAL.Models_2;

public partial class EventParticipant
{
    public int EventId { get; set; }

    public int UserId { get; set; }

    public decimal Role { get; set; }

    public virtual LabEvent Event { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
