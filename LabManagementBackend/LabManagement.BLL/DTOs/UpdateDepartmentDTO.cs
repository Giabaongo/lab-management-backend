using System.ComponentModel.DataAnnotations;

namespace LabManagement.BLL.DTOs
{
    public class UpdateDepartmentDTO
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsPublic { get; set; }
    }
}
