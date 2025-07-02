using MoviesAPI.Auth;
using MoviesAPI.Mapping;
using MoviesApplication.Models;
using MoviesApplication.Services;
using MoviesContracts.Responses;

namespace MoviesAPI.Endpoints.Movies;

public static class GetMovieEndpoint
{
    public const string Name = "GetMovie";

    public static IEndpointRouteBuilder MapGetMovie(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Movies.Get, async (
            string idOrSlug,
            IMovieService movieService,
            HttpContext context,
            CancellationToken cancellationToken = default) =>
        {
            Guid? userId = context.GetUserId();

            Movie? movie = Guid.TryParse(idOrSlug, out var id)
                ? await movieService.GetByIdAsync(id, userId, cancellationToken)
                : await movieService.GetBySlugAsync(idOrSlug, userId, cancellationToken);

            if (movie is null)
            {
                return Results.NotFound();
            }

            MovieResponse response = movie.MapToResponse();

            return TypedResults.Ok(response);
        })
        .WithName(Name);
        
        return app;
    }
}