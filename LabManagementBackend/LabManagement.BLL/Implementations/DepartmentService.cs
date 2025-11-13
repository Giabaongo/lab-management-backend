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
                .Where(d => d.UserDepartments.Any(ud => ud.UserId == userId && ud.Status == (int)Constant.RegistrationStatus.Approved))
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

            // Public departments don't require registration
            if (department.IsPublic)
            {
                throw new BadRequestException("Public departments are accessible to everyone. No registration required.");
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
                if (existingMembership.Status == (int)Constant.RegistrationStatus.Pending)
                {
                    throw new BadRequestException("You already have a pending registration request for this department");
                }
                else if (existingMembership.Status == (int)Constant.RegistrationStatus.Approved)
                {
                    throw new BadRequestException("You are already a member of this department");
                }
                else if (existingMembership.Status == (int)Constant.RegistrationStatus.Rejected)
                {
                    throw new BadRequestException("Your previous registration request was rejected. Please contact an administrator.");
                }
            }

            // Count only approved memberships for the limit check
            var approvedMembershipCount = await _unitOfWork.UserDepartments
                .GetUserDepartmentsQueryable()
                .Where(ud => ud.UserId == userId && ud.Status == (int)Constant.RegistrationStatus.Approved)
                .CountAsync();
                
            if (approvedMembershipCount >= Constant.MaxDepartmentsPerMember)
            {
                throw new BadRequestException($"Members can only be part of {Constant.MaxDepartmentsPerMember} departments");
            }

            var membership = new UserDepartment
            {
                DepartmentId = departmentId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                Status = (int)Constant.RegistrationStatus.Pending // Set to pending by default
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

        public async Task<IEnumerable<DepartmentRegistrationDTO>> GetPendingRegistrationsAsync(int departmentId, int requesterId, Constant.UserRole requesterRole)
        {
            var department = await _unitOfWork.Departments.GetByIdAsync(departmentId);
            if (department == null)
            {
                throw new NotFoundException("Department", departmentId);
            }

            // If LabManager, verify they manage a lab in this department
            if (requesterRole == Constant.UserRole.LabManager)
            {
                var managesLabInDepartment = await _unitOfWork.Labs
                    .ExistsAsync(l => l.ManagerId == requesterId && l.DepartmentId == departmentId);
                
                if (!managesLabInDepartment)
                {
                    throw new UnauthorizedException("You can only view registration requests for departments where you manage a lab");
                }
            }

            var pendingRegistrations = await _unitOfWork.UserDepartments
                .GetUserDepartmentsQueryable()
                .Where(ud => ud.DepartmentId == departmentId && ud.Status == (int)Constant.RegistrationStatus.Pending)
                .Include(ud => ud.User)
                .Include(ud => ud.Department)
                .AsNoTracking()
                .ToListAsync();

            return pendingRegistrations.Select(ud => new DepartmentRegistrationDTO
            {
                UserId = ud.UserId,
                UserName = ud.User.Name,
                Email = ud.User.Email,
                DepartmentId = ud.DepartmentId,
                DepartmentName = ud.Department.Name,
                CreatedAt = ud.CreatedAt,
                Status = ud.Status,
                StatusText = ((Constant.RegistrationStatus)ud.Status).ToString()
            });
        }

        public async Task<bool> ApproveOrRejectRegistrationAsync(int departmentId, int targetUserId, bool approve, int requesterId, Constant.UserRole requesterRole)
        {
            var membership = await _unitOfWork.UserDepartments.GetMembershipAsync(targetUserId, departmentId);
            if (membership == null)
            {
                throw new NotFoundException("Registration request not found");
            }

            if (membership.Status != (int)Constant.RegistrationStatus.Pending)
            {
                throw new BadRequestException("This registration request has already been processed");
            }

            // If LabManager, verify they manage a lab in this department
            if (requesterRole == Constant.UserRole.LabManager)
            {
                var managesLabInDepartment = await _unitOfWork.Labs
                    .ExistsAsync(l => l.ManagerId == requesterId && l.DepartmentId == departmentId);
                
                if (!managesLabInDepartment)
                {
                    throw new UnauthorizedException("You can only approve/reject registrations for departments where you manage a lab");
                }
            }

            if (approve)
            {
                // Check if user already has max departments (only count approved ones)
                var approvedCount = await _unitOfWork.UserDepartments
                    .GetUserDepartmentsQueryable()
                    .Where(ud => ud.UserId == targetUserId && ud.Status == (int)Constant.RegistrationStatus.Approved)
                    .CountAsync();

                if (approvedCount >= Constant.MaxDepartmentsPerMember)
                {
                    throw new BadRequestException($"User already has {Constant.MaxDepartmentsPerMember} approved department memberships");
                }

                membership.Status = (int)Constant.RegistrationStatus.Approved;
            }
            else
            {
                membership.Status = (int)Constant.RegistrationStatus.Rejected;
            }

            await _unitOfWork.UserDepartments.UpdateAsync(membership);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<DepartmentDTO>> GetRegisterableDepartmentsAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException("User", userId);
            }

            // Get all departments that user has already registered to (any status)
            var userDepartmentIds = await _unitOfWork.UserDepartments
                .GetUserDepartmentsQueryable()
                .Where(ud => ud.UserId == userId)
                .Select(ud => ud.DepartmentId)
                .ToListAsync();

            // Check if user has reached the limit
            var approvedCount = await _unitOfWork.UserDepartments
                .GetUserDepartmentsQueryable()
                .Where(ud => ud.UserId == userId && ud.Status == (int)Constant.RegistrationStatus.Approved)
                .CountAsync();

            // Get departments that:
            // - Are NOT public (IsPublic = false)
            // - User has NOT already registered to
            // - Only if user hasn't reached the department limit
            var departments = await _unitOfWork.Departments
                .GetDepartmentsQueryable()
                .Where(d => !d.IsPublic && !userDepartmentIds.Contains(d.DepartmentId))
                .AsNoTracking()
                .ToListAsync();

            var result = _mapper.Map<List<DepartmentDTO>>(departments);
            
            // Add a flag indicating if user can still register
            foreach (var dto in result)
            {
                dto.CanRegister = approvedCount < Constant.MaxDepartmentsPerMember;
            }

            return result;
        }
    }
}
