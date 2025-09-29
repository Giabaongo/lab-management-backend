using System.ComponentModel.DataAnnotations;

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

        [Range(0, 10)]
        public decimal? Role { get; set; }
    }
}