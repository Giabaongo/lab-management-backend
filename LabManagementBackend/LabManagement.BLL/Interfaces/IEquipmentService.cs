using LabManagement.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.BLL.Interfaces
{
    public interface IEquipmentService
    {
        Task<IEnumerable<EquipmentDTO>> GetAllEquipmentAsync();
        Task<EquipmentDTO?> GetEquipmentByIdAsync(int id);
        Task<EquipmentDTO> CreateEquipmentAsync(CreateEquipmentDTO createEquipmentDTO);
        Task<EquipmentDTO?> UpdateEquipmentAsync(int id, UpdateEquipmentDTO updateEquipmentDTO);
        Task<bool> DeleteEquipmentAsync(int id);
        Task<bool> EquipmentCodeExistsAsync(string code);
        Task<bool> EquipmentStatusExistsAsync(int status);
    }
}
