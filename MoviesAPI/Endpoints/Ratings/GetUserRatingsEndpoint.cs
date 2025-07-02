using MoviesAPI.Auth;
using MoviesApplication.Models;
using MoviesApplication.Services;
using MoviesContracts.Responses;

namespace MoviesAPI.Endpoints.Ratings;

public static class GetUserRatingsEndpoint
{
    public const string Name = "GetUserRatings";
    
    public static IEndpointRouteBuilder MapGetUserRatings(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Ratings.GetUserRatings, async (
                IRatingService ratingService,
                HttpContext context,
                CancellationToken cancellationToken = default) =>
            {
                Guid? userId = context.GetUserId();
                
                IEnumerable<MovieRating> ratings = await ratingService.GetRatingsForUserAsync(userId.Value, cancellationToken);
                
                return TypedResults.Ok(ratings);
            })
            .WithName(Name)
            .RequireAuthorization()
            .Produces<MovieRatingResponse>(StatusCodes.Status200OK);
        
        return app;
    }
}