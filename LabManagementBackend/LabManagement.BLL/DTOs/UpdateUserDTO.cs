using System.ComponentModel.DataAnnotations;
using LabManagement.Common.Constants;

namespace LabManagement.BLL.DTOs
{
    public class UpdateUserDTO
    {
        [StringLength(100)]
        public string? Name { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string? Email { get; set; }

        [StringLength(100, MinimumLength = 6)]
        public string? Password { get; set; }

        [EnumDataType(typeof(Constant.UserRole))]
        public Constant.UserRole? Role { get; set; }
    }
}


