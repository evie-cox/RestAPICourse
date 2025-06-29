using Dapper;
using MoviesApplication.Database;

namespace MoviesApplication.Repositories.Internal;

public class RatingRepository : IRatingRepository
{
    private readonly IDBConnectionFactory _dbConnectionFactory;

    public RatingRepository(IDBConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }
    
    public async Task<float?> GetRatingAsync(Guid movieId, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<float?>(new CommandDefinition("""
            select round(avg(r.rating), 1) from ratings r
            where movieid = @movieId
            """, new { movieId }, cancellationToken: cancellationToken));
    }

    public async Task<(float? Rating, int? UserRating)> GetRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<(float?, int?)>(new CommandDefinition("""
            select round(avg(r.rating), 1),
                (select rating
                from ratings
                where movieid = @movieId
                    and userid = @userId
                limit 1)
            where movieid = @movieId
            """, new { movieId, userId }, cancellationToken: cancellationToken));
    }
}