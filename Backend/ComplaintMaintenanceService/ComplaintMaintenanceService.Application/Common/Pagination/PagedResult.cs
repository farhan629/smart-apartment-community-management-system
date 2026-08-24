namespace ComplaintMaintenanceService.Application.Common.Pagination;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int Limit { get; set; }
    public int TotalPages => Limit > 0 ? (int)Math.Ceiling(TotalCount / (double)Limit) : 0;
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
