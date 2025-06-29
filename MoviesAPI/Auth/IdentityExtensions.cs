namespace MoviesAPI.Auth;

public static class IdentityExtensions
{
    public static Guid? GetUserId(this HttpContext httpContext)
    {
        var userId = httpContext.User.Claims.SingleOrDefault(x => x.Type == "userid");

        if (Guid.TryParse(userId?.Value, out var parsedId))
        {
            return parsedId;
        }
        
        return null;
    }
}