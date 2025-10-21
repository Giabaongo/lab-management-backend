using System;
using System.Collections.Generic;

namespace LabManagement.DAL.Models;

public partial class LabZone
{
    public int ZoneId { get; set; }

    public int LabId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    //public DateTime ExpiryTime { get; set; } //việc định nghĩa ExpiryTime đã diễn ra ở Service => k cần phải định nghĩa mapper

    //Giải sửa Voucher là 1 model riêng
    //public Voucher voucher { get; set; } //vd trong Voucher có 1 field là Code: public string Code { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Lab Lab { get; set; } = null!;

    public virtual ICollection<LabEvent> LabEvents { get; set; } = new List<LabEvent>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}
