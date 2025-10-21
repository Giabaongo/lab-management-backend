using System.ComponentModel.DataAnnotations;

namespace LabManagement.BLL.DTOs
{
    public class UpdateBookingDTO
    {
        public int? UserId { get; set; }
        
        public int? LabId { get; set; }
        
        public int? ZoneId { get; set; }
        
        public DateTime? StartTime { get; set; }
        
        public DateTime? EndTime { get; set; }
        
        public int? Status { get; set; }
        
        public string? Notes { get; set; }
    }
}
