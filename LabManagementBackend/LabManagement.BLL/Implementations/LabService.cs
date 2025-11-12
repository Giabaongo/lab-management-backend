using AutoMapper;
using LabManagement.BLL.DTOs;
using LabManagement.BLL.Interfaces;
using LabManagement.Common.Extensions;
using LabManagement.Common.Models;
using LabManagement.DAL.Interfaces;
using LabManagement.DAL.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace LabManagement.BLL.Implementations
{
    public class LabService : ILabService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisHelper _redisHelper;

        public LabService(IMapper mapper, IUnitOfWork unitOfWork, IRedisHelper redisHelper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _redisHelper = redisHelper;
        }

        public async Task<LabDTO> CreateLabAsync(CreateLabDTO createLabDTO)
        {
            var lab = _mapper.Map<Lab>(createLabDTO);
            await _unitOfWork.Labs.AddAsync(lab);
            await _unitOfWork.SaveChangesAsync();

            // 1. INVALIDATE THE CACHE
            await _redisHelper.DeleteAsync("AllLabs");

            return _mapper.Map<LabDTO>(lab);
        }

        public async Task<bool> DeleteLabAsync(int id)
        {
            var lab = await _unitOfWork.Labs.GetByIdAsync(id);
            if (lab == null) return false;

            await _unitOfWork.Labs.DeleteAsync(lab);
            await _unitOfWork.SaveChangesAsync();

            // --- UPDATE HERE ---
            // Invalidate both the list cache and the item cache
            await _redisHelper.DeleteAsync("AllLabs");
            await _redisHelper.DeleteAsync($"Lab:{id}"); // Use the ID passed into the method
                                                         // --- END UPDATE ---

            return true;
        }

        public async Task<IEnumerable<LabDTO>> GetAllLabsAsync()
        {
            // 1. Define a unique key for this data in Redis
            const string cacheKey = "AllLabs";

            // 2. Try to get the data from Redis first
            string? cachedLabsJson = await _redisHelper.GetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedLabsJson))
            {
                // 3. CACHE HIT (FAST PATH)
                // Data was in Redis! Convert it from JSON and return it.
                var cachedLabs = JsonSerializer.Deserialize<IEnumerable<LabDTO>>(cachedLabsJson);
                return cachedLabs;
            }

            // 4. CACHE MISS (SLOW PATH)
            // Data was not in Redis. Run your ORIGINAL code to get it from the database.
            var labsFromDb = await _unitOfWork.Labs.GetAllAsync();
            var labsDTO = _mapper.Map<IEnumerable<LabDTO>>(labsFromDb);

            // 5. NOW, save this data to Redis for next time
            string labsJsonToCache = JsonSerializer.Serialize(labsDTO);
            await _redisHelper.SetAsync(cacheKey, labsJsonToCache, TimeSpan.FromMinutes(30)); // Cache for 30 mins

            // 6. Return the data you got from the database
            return labsDTO;
        }

        public async Task<PagedResult<LabDTO>> GetLabsAsync(QueryParameters queryParams)
        {
            var query = _unitOfWork.Labs.GetLabsQueryable();

            if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
            {
                var term = queryParams.SearchTerm.ToLower();
                query = query.Where(l =>
                    l.Name.ToLower().Contains(term) ||
                    (l.Location != null && l.Location.ToLower().Contains(term)) ||
                    (l.Description != null && l.Description.ToLower().Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(queryParams.SortBy))
            {
                query = queryParams.SortBy.ToLower() switch
                {
                    "name" => queryParams.IsDescending ? query.OrderByDescending(l => l.Name) : query.OrderBy(l => l.Name),
                    "location" => queryParams.IsDescending ? query.OrderByDescending(l => l.Location) : query.OrderBy(l => l.Location),
                    "managerid" => queryParams.IsDescending ? query.OrderByDescending(l => l.ManagerId) : query.OrderBy(l => l.ManagerId),
                    _ => query.OrderBy(l => l.LabId)
                };
            }
            else
            {
                query = query.OrderBy(l => l.LabId);
            }

            var pagedLabs = await query.ToPagedResultAsync(queryParams.PageNumber, queryParams.PageSize);

            return new PagedResult<LabDTO>
            {
                Items = _mapper.Map<IEnumerable<LabDTO>>(pagedLabs.Items),
                PageNumber = pagedLabs.PageNumber,
                PageSize = pagedLabs.PageSize,
                TotalCount = pagedLabs.TotalCount
            };
        }

        public async Task<LabDTO?> GetLabByIdAsync(int id)
        {
            // 1. Define a unique, dynamic key for this specific lab
            string cacheKey = $"Lab:{id}";

            // 2. Try to get the data from Redis first
            string? cachedLabJson = await _redisHelper.GetAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedLabJson))
            {
                // 3. CACHE HIT (FAST PATH)
                // Data was in Redis! Convert it from JSON and return it.
                var cachedLab = JsonSerializer.Deserialize<LabDTO>(cachedLabJson);
                return cachedLab;
            }

            // 4. CACHE MISS (SLOW PATH)
            // Data was not in Redis. Run your ORIGINAL code.
            var labFromDb = await _unitOfWork.Labs.GetByIdAsync(id);

            if (labFromDb == null)
            {
                // Lab doesn't exist, so don't cache anything.
                return null;
            }

            // Lab was found, so map it.
            var labDTO = _mapper.Map<LabDTO>(labFromDb);

            // 5. NOW, save this DTO to Redis for next time
            string labJsonToCache = JsonSerializer.Serialize(labDTO);
            await _redisHelper.SetAsync(cacheKey, labJsonToCache, TimeSpan.FromMinutes(30)); // Cache for 30 mins

            // 6. Return the data you got from the database
            return labDTO;
        }

        public async Task<bool> LabExistsAsync(string name)
        {
            return await _unitOfWork.Labs.ExistsAsync(l => l.Name == name);
        }

        public async Task<LabDTO?> UpdateLabAsync(UpdateLabDTO updateLabDTO, string name)
        {
            var lab = await _unitOfWork.Labs.FirstOrDefaultAsync(l => l.Name == name);
            if (lab != null)
            {
                _mapper.Map(updateLabDTO, lab);
                await _unitOfWork.Labs.UpdateAsync(lab);
                await _unitOfWork.SaveChangesAsync();

                // --- UPDATE HERE ---
                // Invalidate both the list cache and the item cache
                await _redisHelper.DeleteAsync("AllLabs");
                await _redisHelper.DeleteAsync($"Lab:{lab.LabId}"); // Use the ID from the lab object
                                                                    // --- END UPDATE ---

                return _mapper.Map<LabDTO>(lab);
            }
            return null;
        }
    }
}
