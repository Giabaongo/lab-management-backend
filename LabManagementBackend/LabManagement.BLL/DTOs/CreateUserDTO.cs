using System.ComponentModel.DataAnnotations;

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
        [Range(0, 10)]
        public decimal Role { get; set; }
    }
}