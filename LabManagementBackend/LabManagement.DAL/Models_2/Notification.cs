using System;
using System.Collections.Generic;

namespace LabManagement.DAL.Models_2;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int RecipientId { get; set; }

    public int EventId { get; set; }

    public string Message { get; set; } = null!;

    public DateTime SentAt { get; set; }

    public bool IsRead { get; set; }

    public virtual LabEvent Event { get; set; } = null!;

    public virtual User Recipient { get; set; } = null!;
}
