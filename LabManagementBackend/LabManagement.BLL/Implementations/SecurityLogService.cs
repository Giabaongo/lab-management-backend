using AutoMapper;
using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;

namespace LabManagement.BLL.Implementations
{
    public class SecurityLogService : ISecurityLogService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public SecurityLogService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<SecurityLogDTO> CreateSecurityLogAsync(CreateSecurityLogDTO createSecurityLogDTO)
        {
            var securityLog = _mapper.Map<SecurityLog>(createSecurityLogDTO);
            await _unitOfWork.SecurityLogs.AddAsync(securityLog);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<SecurityLogDTO>(securityLog);
        }

        public async Task<bool> DeleteSecurityLogAsync(int id)
        {
            var securityLog = await _unitOfWork.SecurityLogs.GetByIdAsync(id);
            if (securityLog == null) return false;

            await _unitOfWork.SecurityLogs.DeleteAsync(securityLog);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SecurityLogDTO>> GetAllSecurityLogsAsync()
        {
            var securityLogs = await _unitOfWork.SecurityLogs.GetAllAsync();
            return _mapper.Map<IEnumerable<SecurityLogDTO>>(securityLogs);
        }

        public async Task<SecurityLogDTO?> GetSecurityLogByIdAsync(int id)
        {
            var securityLog = await _unitOfWork.SecurityLogs.GetByIdAsync(id);
            return securityLog == null ? null : _mapper.Map<SecurityLogDTO>(securityLog);
        }

        public async Task<bool> SecurityLogExistsAsync(int id)
        {
            return await _unitOfWork.SecurityLogs.ExistsAsync(s => s.LogId == id);
        }

        public async Task<SecurityLogDTO?> UpdateSecurityLogAsync(int id, UpdateSecurityLogDTO updateSecurityLogDTO)
        {
            var securityLog = await _unitOfWork.SecurityLogs.GetByIdAsync(id);
            if (securityLog == null) return null;

            _mapper.Map(updateSecurityLogDTO, securityLog);
            await _unitOfWork.SecurityLogs.UpdateAsync(securityLog);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<SecurityLogDTO>(securityLog);
        }
    }
}
