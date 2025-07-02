using MoviesAPI.Auth;
using MoviesApplication.Services;

namespace MoviesAPI.Endpoints.Ratings;

public static class DeleteRatingEndpoint
{
    public const string Name = "DeleteRating";
    
    public static IEndpointRouteBuilder MapDeleteRating(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.Movies.DeleteRating, async (
                Guid id,
                IRatingService ratingService,
                HttpContext context,
                CancellationToken cancellationToken = default) =>
            {
                Guid? userId = context.GetUserId();
        
                bool result = await ratingService.DeleteRatingAsync(id, userId.Value, cancellationToken);
        
                return result ? TypedResults.Ok() : Results.NotFound();
            })
            .WithName(Name);
        
        return app;
    }
}