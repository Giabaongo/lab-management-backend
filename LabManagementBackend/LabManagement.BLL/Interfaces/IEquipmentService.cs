using LabManagement.BLL.DTOs;
using LabManagement.Common.Models;

namespace LabManagement.BLL.Interfaces
{
    public interface IEquipmentService
    {
        Task<IEnumerable<EquipmentDTO>> GetAllEquipmentAsync();
        Task<PagedResult<EquipmentDTO>> GetEquipmentAsync(QueryParameters queryParams);
        Task<EquipmentDTO?> GetEquipmentByIdAsync(int id);
        Task<EquipmentDTO> CreateEquipmentAsync(CreateEquipmentDTO createEquipmentDTO);
        Task<EquipmentDTO?> UpdateEquipmentAsync(int id, UpdateEquipmentDTO updateEquipmentDTO);
        Task<bool> DeleteEquipmentAsync(int id);
        Task<bool> EquipmentCodeExistsAsync(string code);
        Task<bool> EquipmentStatusExistsAsync(int status);
    }
}
