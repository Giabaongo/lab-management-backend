namespace LabManagement.Common.Models;

/// <summary>
/// Query parameters for list endpoints with search, sort, and pagination
/// </summary>
public class QueryParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    /// <summary>
    /// Page number (starts from 1)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of items per page (max 100)
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    /// <summary>
    /// Search keyword (searches across multiple fields)
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Field name to sort by
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort direction: "asc" or "desc"
    /// </summary>
    public string SortOrder { get; set; } = "asc";

    /// <summary>
    /// Check if sort order is descending
    /// </summary>
    public bool IsDescending => SortOrder?.ToLower() == "desc";
}
