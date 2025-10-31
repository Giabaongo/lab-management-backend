using LabManagement.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.BLL.Interfaces
{
    public interface ILabZoneService
    {
        Task<IEnumerable<LabZoneDTO>> GetAllLabZonesAsync();
        Task<LabZoneDTO?> GetLabZoneByIdAsync(int id);
        Task<LabZoneDTO> CreateLabZoneAsync(CreateLabZoneDTO createLabZoneDTO);
        Task<LabZoneDTO?> UpdateLabZoneAsync(int id, UpdateLabZoneDTO updateLabZoneDTO);
        Task<bool> DeleteLabZoneAsync(int id);
        Task<bool> LabZoneNameExistsAsync(string name);
        Task<bool> LabZoneIdExistsAsync(int id);
    }
}
