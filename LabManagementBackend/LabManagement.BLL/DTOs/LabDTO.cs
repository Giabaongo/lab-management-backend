using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.BLL.DTOs
{
    public class LabDTO
    {
        public int labId { get; set; }
        public string labName { get; set; } = null!;
        public int managerId { get; set; }
        public string location { get; set; } = null!;
        public string description { get; set; } = null!;
        public int departmentId { get; set; }
        public string departmentName { get; set; } = null!;
        public bool isPublic { get; set; }
        public bool isOpen { get; set; }
        public int status { get; set; }
    }
}
