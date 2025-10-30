using LabManagement.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace LabManagement.Common.Extensions;

/// <summary>
/// Extension methods for pagination
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Apply pagination to IQueryable
    /// </summary>
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        int pageNumber,
        int pageSize) where T : class
    {
        var totalCount = await query.CountAsync();
        
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<T>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}
