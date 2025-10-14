using System.ComponentModel.DataAnnotations;
using LabManagement.Common.Constants;

namespace LabManagement.BLL.DTOs
{
    public class CreateUserDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = null!;

        [Required]
        [EnumDataType(typeof(Constant.UserRole))]
        public Constant.UserRole Role { get; set; }
    }
}


