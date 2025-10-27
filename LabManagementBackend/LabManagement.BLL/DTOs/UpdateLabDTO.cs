using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.BLL.DTOs
{
    public class UpdateLabDTO
    {
        [Required]
        [StringLength(100)]
        public string name { get; set; } = null!;

        [Required]
        public int mananger_Id { get; set; }

        [Required]
        [StringLength(100)]
        public string description { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string location { get; set; } = null!;
    }
}
