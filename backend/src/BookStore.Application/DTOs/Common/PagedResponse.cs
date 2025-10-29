namespace BookStore.Application.DTOs.Common;

public class PagedResponse<T>
{
    public PagedResponse(IEnumerable<T> data, int totalRecords, int pageNumber, int pageSize)
    {
        Data = data;
        TotalRecords = totalRecords;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
    }

    public IEnumerable<T> Data { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

