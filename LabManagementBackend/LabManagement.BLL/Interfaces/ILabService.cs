using LabManagement.BLL.DTOs;
using LabManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabManagement.BLL.Interfaces
{
    public interface ILabService
    {
        Task<IEnumerable<LabDTO>> GetAllLabsAsync();
        Task<LabDTO?> GetLabByIdAsync(int id);
        Task<LabDTO> CreateLabAsync(CreateLabDTO createLabDTO);
        Task<LabDTO?> UpdateLabAsync(UpdateLabDTO updateLabDTO, string name);
        Task<bool> DeleteLabAsync(int id);
        Task<bool> LabExistsAsync(string name);
    }
}
