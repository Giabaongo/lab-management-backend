using System;
using System.Collections.Generic;

namespace LabManagement.DAL.Models;

public partial class Department
{
    public int DepartmentId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsPublic { get; set; }

    public virtual ICollection<Lab> Labs { get; set; } = new List<Lab>();

    public virtual ICollection<UserDepartment> UserDepartments { get; set; } = new List<UserDepartment>();
}
