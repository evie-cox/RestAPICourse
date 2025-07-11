using FluentValidation;
using MoviesApplication.Database;
using MoviesApplication.Models;
using MoviesApplication.Repositories;

namespace MoviesApplication.Services.Internal;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IRatingRepository _ratingRepository;
    private readonly IValidator<Movie> _movieValidator;
    private readonly IValidator<GetAllMoviesOptions> _optionsValidator;

    public MovieService(
        IMovieRepository movieRepository,
        IRatingRepository ratingRepository,
        IValidator<Movie> movieValidator,
        IValidator<GetAllMoviesOptions> optionsValidator)
    {
        _movieRepository = movieRepository;
        _ratingRepository = ratingRepository;
        _movieValidator = movieValidator;
        _optionsValidator = optionsValidator;
    }

    public async Task<bool> CreateAsync(Movie movie, CancellationToken cancellationToken = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, cancellationToken);
        return await _movieRepository.CreateAsync(movie, cancellationToken);
    }

    public Task<Movie?> GetByIdAsync(Guid id, Guid? userId = default, CancellationToken cancellationToken = default)
    {
        return _movieRepository.GetByIdAsync(id, userId, cancellationToken);
    }

    public Task<Movie?> GetBySlugAsync(string slug, Guid? userId = default, CancellationToken cancellationToken = default)
    {
        return _movieRepository.GetBySlugAsync(slug, userId, cancellationToken);
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options, CancellationToken cancellationToken = default)
    {
        await _optionsValidator.ValidateAndThrowAsync(options, cancellationToken);
        
        return await _movieRepository.GetAllAsync(options, cancellationToken);
    }

    public async Task<Movie?> UpdateAsync(Movie movie, Guid? userId = default, CancellationToken cancellationToken = default)
    {
        await _movieValidator.ValidateAndThrowAsync(movie, cancellationToken);

        var movieExists = await _movieRepository.ExistsByIdAsync(movie.Id, cancellationToken);

        if (!movieExists)
        {
            return null;
        }
        
        await _movieRepository.UpdateAsync(movie, cancellationToken);

        if (!userId.HasValue)
        {
            var rating = await _ratingRepository.GetRatingAsync(movie.Id, cancellationToken);
            movie.Rating = rating;
            return movie;
        }
        
        var ratings = await _ratingRepository.GetRatingAsync(movie.Id, userId.Value, cancellationToken);
        movie.Rating = ratings.Rating;
        movie.UserRating = ratings.UserRating;
        return movie;
    }

    public Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _movieRepository.DeleteByIdAsync(id, cancellationToken);
    }

    public Task<int> GetCountAsync(string? title, int? yearOfRelease, CancellationToken cancellationToken = default)
    {
        return _movieRepository.GetCountAsync(title, yearOfRelease, cancellationToken);
    }
}