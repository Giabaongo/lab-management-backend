using System.ComponentModel.DataAnnotations;

namespace LabManagement.BLL.DTOs
{
    public class CreateSecurityLogDTO
    {
        [Required(ErrorMessage = "EventId is required")]
        public int EventId { get; set; }

        [Required(ErrorMessage = "SecurityId is required")]
        public int SecurityId { get; set; }

        [Required(ErrorMessage = "ActionType is required")]
        public int ActionType { get; set; }

        public string? PhotoUrl { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }
    }

    public class UpdateSecurityLogDTO
    {
        public int? EventId { get; set; }

        public int? SecurityId { get; set; }

        public int? ActionType { get; set; }

        public string? PhotoUrl { get; set; }

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }
    }
}
