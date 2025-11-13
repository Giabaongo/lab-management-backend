using System;

namespace LabManagement.BLL.DTOs
{
    public class DepartmentRegistrationDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int Status { get; set; }
        public string StatusText { get; set; } = null!;
    }

    public class ApproveRegistrationDTO
    {
        public int UserId { get; set; }
        public bool Approve { get; set; } // true = approve, false = reject
    }
}
