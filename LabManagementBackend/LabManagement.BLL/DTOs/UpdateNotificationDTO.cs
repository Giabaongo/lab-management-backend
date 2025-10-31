using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.BLL.DTOs
{
    public class UpdateNotificationDTO
    {
        [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters")]
        public string? Message { get; set; }

        public bool? IsRead { get; set; }
    }
}
