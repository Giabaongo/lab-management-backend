using System;
using System.Collections.Generic;

namespace LabManagement.DAL.Models;

public partial class UserDepartment
{
    public int UserId { get; set; }

    public int DepartmentId { get; set; }

    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Registration status: 0 = Pending, 1 = Approved, 2 = Rejected
    /// </summary>
    public int Status { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
