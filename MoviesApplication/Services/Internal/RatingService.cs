using FluentValidation;
using FluentValidation.Results;
using MoviesApplication.Models;
using MoviesApplication.Repositories;

namespace MoviesApplication.Services.Internal;

public class RatingService : IRatingService
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IMovieRepository _movieRepository;

    public RatingService(
        IRatingRepository ratingRepository,
        IMovieRepository movieRepository)
    {
        _ratingRepository = ratingRepository;
        _movieRepository = movieRepository;
    }

    public async Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken cancellationToken = default)
    {
        if (rating <= 0 || rating > 5)
        {
            throw new ValidationException(new[]
            {
                new ValidationFailure(
                    propertyName: "Rating",
                    errorMessage: "Rating must be between 1 and 5")
            });
        }
        
        var movieExists = await _movieRepository.ExistsByIdAsync(movieId, cancellationToken);

        if (!movieExists)
        {
            return false;
        }
        
        return await _ratingRepository.RateMovieAsync(movieId, rating, userId, cancellationToken);
    }

    public Task<bool> DeleteRatingAsync(Guid movieId, Guid userId, CancellationToken cancellationToken = default)
    {
        return _ratingRepository.DeleteRatingAsync(movieId, userId, cancellationToken);
    }

    public Task<IEnumerable<MovieRating>> GetRatingsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _ratingRepository.GetRatingsForUserAsync(userId, cancellationToken);
    }
}