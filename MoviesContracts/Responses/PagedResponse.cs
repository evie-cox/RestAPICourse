namespace MoviesContracts.Responses;

public class PagedResponse<TResponse>
{
    public required IEnumerable<MovieResponse> Movies { get; init; } = Enumerable.Empty<MovieResponse>();

    public required int PageSize { get; init; }
    
    public required int PageNumber { get; init; }
    
    public required int TotalCount { get; init; }
    
    public bool HasNextPage => TotalCount > (PageNumber * PageSize);
}