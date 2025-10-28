using System;
using System.Collections.Generic;

namespace LabManagement.DAL.Models;

public partial class Equipment
{
    public int EquipmentId { get; set; }

    public int LabId { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? Description { get; set; }

    public int Status { get; set; }

    public virtual Lab Lab { get; set; } = null!;
}
