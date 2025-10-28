using System;
using System.Collections.Generic;

namespace LabManagement.DAL.Models;

public partial class Lab
{
    public int LabId { get; set; }

    public string Name { get; set; } = null!;

    public int ManagerId { get; set; }

    public string? Location { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();

    public virtual ICollection<LabEvent> LabEvents { get; set; } = new List<LabEvent>();

    public virtual ICollection<LabZone> LabZones { get; set; } = new List<LabZone>();

    public virtual User Manager { get; set; } = null!;

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}
