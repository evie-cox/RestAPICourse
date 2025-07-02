using MoviesContracts.Requests;
using MoviesContracts.Responses;
using Refit;

namespace MoviesAPI.SDK;

[Headers("Authorisation: Bearer")]
public interface IMoviesApi
{
    [Get(ApiEndpoints.Movies.Get)]
    Task<MovieResponse> GetMovieAsync(string idOrSlug);
    
    [Get(ApiEndpoints.Movies.GetAll)]
    Task<MoviesResponse> GetAllMoviesAsync(GetAllMoviesRequest request);
    
    [Post(ApiEndpoints.Movies.Create)]
    Task<MovieResponse> CreateMovieAsync(CreateMovieRequest request);
    
    [Put(ApiEndpoints.Movies.Update)]
    Task<MovieResponse> UpdateMovieAsync(Guid id, UpdateMovieRequest request);
    
    [Delete(ApiEndpoints.Movies.Delete)]
    Task<MovieResponse> DeleteMovieAsync(Guid id);
    
    [Put(ApiEndpoints.Movies.Rate)]
    Task RateMovieAsync(Guid id, RateMovieRequest request);
    
    [Delete(ApiEndpoints.Movies.DeleteRating)]
    Task DeleteRatingAsync(Guid id);
    
    [Get(ApiEndpoints.Ratings.GetUserRatings)]
    Task<IEnumerable<MovieRatingResponse>> GetUserRatingsAsync(Guid id);
}