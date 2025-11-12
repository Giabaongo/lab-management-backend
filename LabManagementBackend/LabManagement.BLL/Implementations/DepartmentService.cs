using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.Common.Constants;
using LabManagement.Common.Exceptions;
using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace LabManagement.BLL.Implementations
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DepartmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<DepartmentDTO> CreateDepartmentAsync(CreateDepartmentDTO createDto)
        {
            if (await _unitOfWork.Departments.NameExistsAsync(createDto.Name))
            {
                throw new BadRequestException($"Department '{createDto.Name}' already exists");
            }

            var department = _mapper.Map<Department>(createDto);
            await _unitOfWork.Departments.AddAsync(department);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<DepartmentDTO>(department);
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id);
            if (department == null)
            {
                return false;
            }

            var hasLabs = await _unitOfWork.Labs.ExistsAsync(l => l.DepartmentId == id);
            if (hasLabs)
            {
                throw new BadRequestException("Cannot delete department while labs are still assigned to it");
            }

            await _unitOfWork.Departments.DeleteAsync(department);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DepartmentExistsAsync(int id)
        {
            return await _unitOfWork.Departments.ExistsAsync(d => d.DepartmentId == id);
        }

        public async Task<IEnumerable<DepartmentDTO>> GetDepartmentsAsync(int? userId = null)
        {
            var departments = await _unitOfWork.Departments
                .GetDepartmentsQueryable()
                .Include(d => d.UserDepartments)
                .AsNoTracking()
                .ToListAsync();

            return MapWithMembership(departments, userId);
        }

        public async Task<IEnumerable<DepartmentDTO>> GetDepartmentsForUserAsync(int userId)
        {
            var departments = await _unitOfWork.Departments
                .GetDepartmentsQueryable()
                .Where(d => d.UserDepartments.Any(ud => ud.UserId == userId))
                .Include(d => d.UserDepartments)
                .AsNoTracking()
                .ToListAsync();

            return MapWithMembership(departments, userId);
        }

        public async Task<DepartmentDTO?> GetDepartmentByIdAsync(int id, int? userId = null)
        {
            var department = await _unitOfWork.Departments
                .GetDepartmentsQueryable()
                .Include(d => d.UserDepartments)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DepartmentId == id);

            if (department == null)
            {
                return null;
            }

            var dto = _mapper.Map<DepartmentDTO>(department);
            if (userId.HasValue)
            {
                dto.IsUserMember = department.UserDepartments.Any(ud => ud.UserId == userId.Value);
            }

            return dto;
        }

        public async Task RegisterUserToDepartmentAsync(int userId, int departmentId, Constant.UserRole role)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(departmentId);
            if (department == null)
            {
                throw new NotFoundException("Department", departmentId);
            }

            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User", userId);
            }

            if (role != Constant.UserRole.Member)
            {
                throw new BadRequestException("Only members can register to departments");
            }

            var existingMembership = await _unitOfWork.UserDepartments.GetMembershipAsync(userId, departmentId);
            if (existingMembership != null)
            {
                throw new BadRequestException("You are already registered to this department");
            }

            var membershipCount = await _unitOfWork.UserDepartments.CountByUserAsync(userId);
            if (membershipCount >= Constant.MaxDepartmentsPerMember)
            {
                throw new BadRequestException($"Members can only register to {Constant.MaxDepartmentsPerMember} departments");
            }

            var membership = new UserDepartment
            {
                DepartmentId = departmentId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.UserDepartments.AddAsync(membership);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task UnregisterUserFromDepartmentAsync(int userId, int departmentId)
        {
            var membership = await _unitOfWork.UserDepartments.GetMembershipAsync(userId, departmentId);
            if (membership == null)
            {
                throw new NotFoundException("Department membership", departmentId);
            }

            await _unitOfWork.UserDepartments.DeleteAsync(membership);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<DepartmentDTO?> UpdateDepartmentAsync(int id, UpdateDepartmentDTO updateDto)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(id);
            if (department == null)
            {
                return null;
            }

            if (await _unitOfWork.Departments.NameExistsAsync(updateDto.Name, id))
            {
                throw new BadRequestException($"Department '{updateDto.Name}' already exists");
            }

            _mapper.Map(updateDto, department);
            await _unitOfWork.Departments.UpdateAsync(department);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<DepartmentDTO>(department);
        }

        private IEnumerable<DepartmentDTO> MapWithMembership(IEnumerable<Department> departments, int? userId)
        {
            var result = _mapper.Map<List<DepartmentDTO>>(departments);
            if (userId.HasValue)
            {
                var membershipLookup = departments
                    .Where(d => d.UserDepartments.Any(ud => ud.UserId == userId.Value))
                    .Select(d => d.DepartmentId)
                    .ToHashSet();

                foreach (var dto in result)
                {
                    dto.IsUserMember = membershipLookup.Contains(dto.DepartmentId);
                }
            }

            return result;
        }
    }
}
