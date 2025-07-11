using Dapper;
using MoviesApplication.Database;
using MoviesApplication.Models;

namespace MoviesApplication.Repositories.Internal;

public class RatingRepository : IRatingRepository
{
    private readonly IDBConnectionFactory _dbConnectionFactory;

    public RatingRepository(IDBConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

        var result = await connection.ExecuteAsync(new CommandDefinition(commandText:"""
                                                                                     insert into ratings(userid, movieid, rating)
                                                                                     values (@userId, @movieId, @rating)
                                                                                     on conflict (userid, movieid) do update
                                                                                        set rating = @rating
                                                                                     """, new { userId, movieId, rating }, cancellationToken: cancellationToken));
        
        return result > 0;
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

    public async Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        
        var result = await connection.ExecuteAsync(new CommandDefinition("""
             delete from ratings
             where movieid = @movieId
             and userid = @userId
             """,  new { movieId, userId }, cancellationToken: cancellationToken));
        
        return result > 0;
    }

    public async Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        
        return await connection.QueryAsync<MovieRating>(new CommandDefinition("""
              select r.rating, r.movieid, m.slug
              from ratings r
              inner join movies m on r.movieid = m.id
              where userid = @userId
              """, new  { userId }, cancellationToken: cancellationToken));
    }
}