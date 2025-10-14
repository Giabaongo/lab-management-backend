using LabManagement.Common.Constants;

namespace LabManagement.BLL.DTOs
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public Constant.UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}


