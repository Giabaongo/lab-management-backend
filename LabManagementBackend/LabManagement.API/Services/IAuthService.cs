using LabManagement.API.DTOs;

namespace LabManagement.API.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> Login(LoginDTO loginDTO);
    }
}
