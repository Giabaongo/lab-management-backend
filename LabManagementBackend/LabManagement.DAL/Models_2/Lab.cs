using System;
using System.Collections.Generic;

namespace LabManagement.DAL.Models_2;

public partial class Lab
{
    public int LabId { get; set; }

    public string Name { get; set; } = null!;

    public int ManagerId { get; set; }

    public string? Location { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<LabEvent> LabEvents { get; set; } = new List<LabEvent>();

    public virtual ICollection<LabZone> LabZones { get; set; } = new List<LabZone>();

    public virtual User Manager { get; set; } = null!;

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}
