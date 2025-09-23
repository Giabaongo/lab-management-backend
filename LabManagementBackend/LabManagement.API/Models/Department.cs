using System;
using System.Collections.Generic;

namespace LabManagement.API.Models;

public partial class Department
{
    public int DepartmentId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Lab> Labs { get; set; } = new List<Lab>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
