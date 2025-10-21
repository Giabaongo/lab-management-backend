using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.BLL.DTOs
{
    public class LabZoneDTO
    {
        public int ZoneId { get; set; }

        public int LabId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

       // public string VoucherCode { get; set; } // dùng tới mapper
    }
}
