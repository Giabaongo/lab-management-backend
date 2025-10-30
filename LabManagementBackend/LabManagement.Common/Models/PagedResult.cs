namespace LabManagement.Common.Models;

/// <summary>
/// Paginated response wrapper with metadata
/// </summary>
public class PagedResult<T>
{
    /// <summary>
    /// List of items in current page
    /// </summary>
    public IEnumerable<T> Items { get; set; } = new List<T>();

    /// <summary>
    /// Current page number
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items across all pages
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Has previous page
    /// </summary>
    public bool HasPrevious => PageNumber > 1;

    /// <summary>
    /// Has next page
    /// </summary>
    public bool HasNext => PageNumber < TotalPages;
}
