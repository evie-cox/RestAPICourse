namespace MoviesContracts.Requests;

public class PagedRequest
{
    public required int PageNumber { get; init; } = 1;
    
    public required int PageSize { get; init; } = 10;
}