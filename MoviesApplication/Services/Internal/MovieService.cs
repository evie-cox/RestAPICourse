using FluentValidation;
using MoviesApplication.Database;
using MoviesApplication.Models;
using MoviesApplication.Repositories;

namespace MoviesApplication.Services.Internal;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IValidator<Movie> _movieValidator;

    public MovieService(
        IMovieRepository movieRepository,
        IValidator<Movie> movieValidator)
    {
        _movieRepository = movieRepository;
        _movieValidator = movieValidator;
    }

    public async Task<bool> CreateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, cancellationToken);
        return await _movieRepository.CreateAsync(movie, cancellationToken);
    }

    public Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _movieRepository.GetByIdAsync(id, cancellationToken);
    }

    public Task<Movie?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return _movieRepository.GetBySlugAsync(slug, cancellationToken);
    }

    public Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _movieRepository.GetAllAsync(cancellationToken);
    }

    public async Task<Movie?> UpdateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, cancellationToken);

        var movieExists = await _movieRepository.ExistsByIdAsync(movie.Id, cancellationToken);

        if (!movieExists)
        {
            return null;
        }
        
        await _movieRepository.UpdateAsync(movie, cancellationToken);
        
        return movie;
    }

    public Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _movieRepository.DeleteByIdAsync(id, cancellationToken);
    }
}