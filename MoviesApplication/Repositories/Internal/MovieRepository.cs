using Dapper;
using MoviesApplication.Database;
using MoviesApplication.Models;

namespace MoviesApplication.Repositories.Internal
{
    public class MovieRepository : IMovieRepository
    {
        private readonly IDBConnectionFactory _dbConnectionFactory;

        public MovieRepository(IDBConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<bool> CreateAsync(Movie movie, CancellationToken cancellationToken = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
            using var transaction = connection.BeginTransaction();

            var result = await connection.ExecuteAsync(new CommandDefinition(commandText: """
                                                                                insert into movies (id, slug, title, yearofrelease)
                                                                                values (@Id, @Slug, @Title, @YearOfRelease)
                                                                                """, movie,
                                                                                cancellationToken: cancellationToken));

            if (result > 0)
            {
                foreach (var genre in movie.Genres)
                {
                    await connection.ExecuteAsync(new CommandDefinition(commandText:"""
                                                                        insert into genres (movieId, name)
                                                                        values (@MovieId, @Name)
                                                                        """, new { MovieId = movie.Id, Name = genre },
                                                                        cancellationToken: cancellationToken));
                }
            }
            
            transaction.Commit();
            
            return result > 0;
        }

        public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

            var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
                new CommandDefinition(commandText: """
                                          select * from movies where id = @id
                                      """, new { id },
                                      cancellationToken: cancellationToken));

            if (movie is null)
            {
                return null;
            }
            
            var genres = await connection.QueryAsync<string>(
                new CommandDefinition(commandText:"""
                    select name from genres where movieId = @id
                """, new { id },
                cancellationToken: cancellationToken));

            foreach (var genre in genres)
            {
                movie.Genres.Add(genre);
            }
            
            return movie;
        }

        public async Task<Movie?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

            var movie = await connection.QuerySingleOrDefaultAsync<Movie>(
                new CommandDefinition(commandText:"""
                                          select * from movies where slug = @slug
                                      """, new { slug },
                                     cancellationToken: cancellationToken));

            if (movie is null)
            {
                return null;
            }
            
            var genres = await connection.QueryAsync<string>(
                new CommandDefinition(commandText:"""
                                      select name from genres where movieId = @id
                                      """, new { id = movie.Id },
                                      cancellationToken: cancellationToken));

            foreach (var genre in genres)
            {
                movie.Genres.Add(genre);
            }
            
            return movie;
        }

        public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);

            var result = await connection.QueryAsync(new CommandDefinition(commandText:"""
                                                                            select m.*, string_agg(g.name, ',') as genres
                                                                            from movies m left join genres g on m.id = g.movieId
                                                                            group by id
                                                                            """,
                                                                            cancellationToken: cancellationToken));

            return result.Select(x => new Movie
            {
                Id = x.id,
                Title = x.title,
                YearOfRelease = x.yearofrelease,
                Genres = Enumerable.ToList(x.genres.Split(',')),
            });
        }

        public async Task<bool> UpdateAsync(Movie movie, CancellationToken cancellationToken = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
            using var transaction = connection.BeginTransaction();
            
            await connection.ExecuteAsync(new CommandDefinition(commandText:"""
                                                                delete from genres where movieId = @id
                                                                """, new { id  = movie.Id },
                                                                cancellationToken: cancellationToken));

            foreach (var genre in movie.Genres)
            {
                await connection.ExecuteAsync(new CommandDefinition(commandText:"""
                                                                    insert into genres (movieId, name)
                                                                    values (@MovieId, @Name)
                                                                    """, new { @MovieId = movie.Id, @Name = genre },
                                                                    cancellationToken: cancellationToken));
            }

            var result = await connection.ExecuteAsync(new CommandDefinition(commandText:"""
                                                                             update movies set slug = @Slug, title = @Title, yearofrelease = @YearOfRelease
                                                                             where id = @Id
                                                                             """, movie,
                                                                             cancellationToken: cancellationToken));
            
            transaction.Commit();
            
            return result > 0;
        }
        
        public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
            using var transaction = connection.BeginTransaction();
            
            await connection.ExecuteAsync(new CommandDefinition(commandText:"""
                                                                delete from genres where movieId = @id
                                                                """, new { id },
                                                                cancellationToken: cancellationToken));

            var result = await connection.ExecuteAsync(new CommandDefinition(commandText:"""
                                                                                delete from movies where id = @id
                                                                                """,  new { id },
                                                                                cancellationToken: cancellationToken));
            transaction.Commit();
            
            return result > 0;
        }

        public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            using var connection = await _dbConnectionFactory.CreateConnectionAsync(cancellationToken);
            
            return await connection.ExecuteScalarAsync<bool>(new CommandDefinition(commandText:"""
                                                                                   select count(1) from movies where id = @id
                                                                                   """, new { id },
                                                                                   cancellationToken: cancellationToken));
        }
    }
}
