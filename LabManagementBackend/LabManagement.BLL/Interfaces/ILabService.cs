using LabManagement.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.BLL.Interfaces
{
    public interface ILabService
    {
        Task<LabDTO> createLab(LabDTO labDTO);
        Task<LabDTO> getLab(int id);
        Task<LabDTO> getAllLabs(LabDTO labDTO);
        Task<LabDTO> updateLab(LabDTO labDTO);
        
    }
}
