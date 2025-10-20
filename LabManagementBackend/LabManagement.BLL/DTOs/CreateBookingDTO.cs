using System.ComponentModel.DataAnnotations;

namespace LabManagement.BLL.DTOs
{
    public class CreateBookingDTO
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int LabId { get; set; }
        
        [Required]
        public int ZoneId { get; set; }
        
        [Required]
        public DateTime StartTime { get; set; }
        
        [Required]
        public DateTime EndTime { get; set; }
        
        [Required]
        public int Status { get; set; }
        
        public string? Notes { get; set; }
    }
}
