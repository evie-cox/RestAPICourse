using MoviesAPI.Auth;
using MoviesApplication.Services;
using MoviesContracts.Requests;

namespace MoviesAPI.Endpoints.Ratings;

public static class RateMovieEndpoint
{
    public const string Name = "RateMovie";
    
    public static IEndpointRouteBuilder MapRateMovie(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Movies.Rate, async (
                Guid id,
                RateMovieRequest request,
                IRatingService ratingService,
                HttpContext context,
                CancellationToken cancellationToken = default) =>
            {
                Guid? userId = context.GetUserId();
        
                bool result = await ratingService.RateMovieAsync(id, request.Rating, userId.Value, cancellationToken);
        
                return result ? TypedResults.Ok() : Results.NotFound();
            })
            .WithName(Name)
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
        
        return app;
    }

}