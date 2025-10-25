using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.BLL.DTOs
{
    public class CreateNotificationDTO
    {
        [Required]
        public int RecipientId { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters")]
        public string Message { get; set; } = null!;


        public bool IsRead { get; set; } = false;
    }
}
