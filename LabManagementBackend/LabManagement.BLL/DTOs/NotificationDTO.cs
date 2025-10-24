using LabManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.BLL.DTOs
{
    public class NotificationDTO
    {
        public int NotificationId { get; set; }

        public int RecipientId { get; set; }

        public int EventId { get; set; }

        public string Message { get; set; } = null!;

        public DateTime SentAt { get; set; }

        public bool IsRead { get; set; }

        public LabEvent EventID { get; set; } = null!;

        public User RecipientID { get; set; } = null!;


    }
}
