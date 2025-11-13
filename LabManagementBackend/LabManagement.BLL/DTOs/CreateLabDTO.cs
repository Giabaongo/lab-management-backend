using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.BLL.DTOs
{
    public class CreateLabDTO
    {

        [Required]
        [StringLength(100)]
        public string name { get; set; } = null!;

        [Required]
        public int managerId { get; set; }

        [Required]
        public int departmentId { get; set; }

        [Required]
        [StringLength(100)]
        public string description { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string location { get; set; } = null!;

        // Default to open and active when creating new lab
        public bool isOpen { get; set; } = true;
        
        public int status { get; set; } = 1; // 1 = Active
    }
}
