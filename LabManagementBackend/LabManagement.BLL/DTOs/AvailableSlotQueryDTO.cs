using System;
using System.ComponentModel.DataAnnotations;

namespace LabManagement.BLL.DTOs
{
    public class AvailableSlotQueryDTO
    {
        [Required]
        public int LabId { get; set; }

        [Required]
        public int ZoneId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Range(1, 480)]
        public int SlotDurationMinutes { get; set; } = 60;

        public TimeSpan DayStart { get; set; } = TimeSpan.FromHours(7);

        public TimeSpan DayEnd { get; set; } = TimeSpan.FromHours(19);
    }
}
