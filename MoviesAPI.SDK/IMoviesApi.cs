using MoviesContracts.Requests;
using MoviesContracts.Responses;
using Refit;

namespace MoviesAPI.SDK;

public interface IMoviesApi
{
    [Get(ApiEndpoints.Movies.Get)]
    Task<MovieResponse> GetMovieAsync(string idOrSlug);
    
    [Get(ApiEndpoints.Movies.GetAll)]
    Task<MoviesResponse> GetAllMoviesAsync(GetAllMoviesRequest request);
}