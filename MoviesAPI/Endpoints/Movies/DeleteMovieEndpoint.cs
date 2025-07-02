using Microsoft.AspNetCore.OutputCaching;
using MoviesAPI.Auth;
using MoviesAPI.Mapping;
using MoviesApplication.Models;
using MoviesApplication.Services;
using MoviesContracts.Responses;

namespace MoviesAPI.Endpoints.Movies;

public static class DeleteMovieEndpoint
{
    public const string Name = "DeleteMovie";

    public static IEndpointRouteBuilder MapDeleteMovie(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.Movies.Delete, async (
                Guid id,
                IMovieService movieService,
                IOutputCacheStore outputCacheStore,
                CancellationToken cancellationToken = default) =>
            {
                bool deleted = await movieService.DeleteByIdAsync(id, cancellationToken);

                if (!deleted)
                {
                    return Results.NotFound();
                }

                await outputCacheStore.EvictByTagAsync("movies", cancellationToken);
            
                return TypedResults.Ok();
            })
            .WithName(Name);
        
        return app;
    }
}