using Microsoft.AspNetCore.OutputCaching;
using MoviesAPI.Mapping;
using MoviesApplication.Models;
using MoviesApplication.Services;
using MoviesContracts.Requests;
using MoviesContracts.Responses;

namespace MoviesAPI.Endpoints.Movies;

public static class CreateMovieEndpoint
{
    public const string Name = "CreateMovie";

    public static IEndpointRouteBuilder MapCreateMovie(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Movies.Create, async (
            CreateMovieRequest request,
            IMovieService movieService,
            IOutputCacheStore outputCacheStore,
            CancellationToken cancellationToken = default) =>
        {
            Movie movie = request.MapToMovie();

            await movieService.CreateAsync(movie);

            await outputCacheStore.EvictByTagAsync("movies", cancellationToken);
            
            MovieResponse response = movie.MapToResponse();
            
            return TypedResults.CreatedAtRoute(response, GetMovieEndpoint.Name, new { idOrSlug = response.Id });
        })
        .WithName($"{Name}V1")
        .Produces<MovieResponse>(StatusCodes.Status201Created)
        .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest);

        return app;
    }
}