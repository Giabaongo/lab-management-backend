using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.BLL.DTOs
{
    public class CreateEquipmentDTO
    {
        public int EquipmentId { get; set; }

        public int LabId { get; set; }

        public string Name { get; set; } = null!;

        public string Code { get; set; } = null!;

        public string? Description { get; set; }

        public int Status { get; set; }
    }
}
