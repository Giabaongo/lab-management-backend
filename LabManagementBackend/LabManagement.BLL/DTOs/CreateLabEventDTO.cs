using System.ComponentModel.DataAnnotations;

namespace LabManagement.BLL.DTOs
{
    public class CreateLabEventDTO
    {
        [Required(ErrorMessage = "LabId is required")]
        public int LabId { get; set; }

        [Required(ErrorMessage = "ZoneId is required")]
        public int ZoneId { get; set; }

        [Required(ErrorMessage = "ActivityTypeId is required")]
        public int ActivityTypeId { get; set; }

        [Required(ErrorMessage = "OrganizerId is required")]
        public int OrganizerId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = null!;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "StartTime is required")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "EndTime is required")]
        public DateTime EndTime { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public int Status { get; set; }

        // Default to false if not provided by frontend
        public bool IsHighPriority { get; set; } = false;
    }

    public class UpdateLabEventDTO
    {
        public int? LabId { get; set; }

        public int? ZoneId { get; set; }

        public int? ActivityTypeId { get; set; }

        public int? OrganizerId { get; set; }

        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string? Title { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int? Status { get; set; }

        // Nullable to allow frontend to omit this field
        public bool? IsHighPriority { get; set; }
    }
}
