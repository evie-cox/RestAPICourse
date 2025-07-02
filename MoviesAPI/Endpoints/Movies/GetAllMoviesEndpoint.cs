using MoviesAPI.Auth;
using MoviesAPI.Mapping;
using MoviesApplication.Models;
using MoviesApplication.Services;
using MoviesContracts.Requests;
using MoviesContracts.Responses;

namespace MoviesAPI.Endpoints.Movies;

public static class GetAllMoviesEndpoint
{
    public const string Name = "GetAllMovies";

    public static IEndpointRouteBuilder MapGetAllMovies(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Movies.GetAll, async (
            [AsParameters] GetAllMoviesRequest request,
            IMovieService movieService,
            HttpContext context,
            CancellationToken cancellationToken = default) =>
        {
            Guid? userId = context.GetUserId();

            var options = request
                .MapToOptions()
                .WithUser(userId);
            
            IEnumerable<Movie> movies = await movieService.GetAllAsync(options, cancellationToken);
            
            int movieCount = await movieService.GetCountAsync(options.Title, options.YearOfRelease, cancellationToken);

            MoviesResponse moviesReponse = movies.MapToResponse(
                request.PageNumber.GetValueOrDefault(PagedRequest.DefaultPageNumber),
                request.PageSize.GetValueOrDefault(PagedRequest.DefaultPageSize),
                movieCount);

            return TypedResults.Ok(moviesReponse);
        })
        .WithName(Name);
        
        return app;
    }
}