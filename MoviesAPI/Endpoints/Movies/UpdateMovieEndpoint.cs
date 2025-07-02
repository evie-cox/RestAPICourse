using Microsoft.AspNetCore.OutputCaching;
using MoviesAPI.Auth;
using MoviesAPI.Mapping;
using MoviesApplication.Models;
using MoviesApplication.Services;
using MoviesContracts.Requests;
using MoviesContracts.Responses;

namespace MoviesAPI.Endpoints.Movies;

public static class UpdateMovieEndpoint
{
    public const string Name = "UpdateMovie";

    public static IEndpointRouteBuilder MapUpdateMovie(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Movies.Update, async (
                Guid id,
                UpdateMovieRequest request,
                IMovieService movieService,
                IOutputCacheStore outputCacheStore,
                HttpContext context,
                CancellationToken cancellationToken = default) =>
            {
                Guid? userId = context.GetUserId();

                Movie movie = request.MapToMovie(id);

                Movie? updatedMovie = await movieService.UpdateAsync(movie, userId, cancellationToken);

                if (updatedMovie is null)
                {
                    return Results.NotFound();
                }

                MovieResponse response = updatedMovie.MapToResponse();

                await outputCacheStore.EvictByTagAsync("movies", cancellationToken);

                return TypedResults.Ok(response);
            })
            .WithName(Name)
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName)
            .Produces<MovieResponse>(StatusCodes.Status200OK)
            .Produces<ValidationFailureResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }
}