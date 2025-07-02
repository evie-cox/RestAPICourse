/*using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Auth;
using MoviesAPI.Mapping;
using MoviesApplication.Models;
using MoviesApplication.Services;
using MoviesContracts.Requests;
using MoviesContracts.Responses;

namespace MoviesAPI.Controllers;

[ApiController]
[ApiVersion(1.0)]
public class RatingsController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingsController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    [Authorize]
    [HttpPut(ApiEndpoints.Movies.Rate)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RateMovie(
        [FromRoute] Guid id,
        [FromBody] RateMovieRequest request,
        CancellationToken cancellationToken = default)
    {
        Guid? userId = HttpContext.GetUserId();
        
        bool result = await _ratingService.RateMovieAsync(id, request.Rating, userId.Value, cancellationToken);
        
        return result ? Ok() : NotFound();
    }

    [Authorize]
    [HttpDelete(ApiEndpoints.Movies.DeleteRating)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteRatingAsync(
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        Guid? userId = HttpContext.GetUserId();
        
        bool result = await _ratingService.DeleteRatingAsync(id, userId.Value, cancellationToken);
        
        return result ? Ok() : NotFound();
    }

    [Authorize]
    [HttpGet(ApiEndpoints.Ratings.GetUserRatings)]
    [ProducesResponseType(typeof(IEnumerable<MovieRatingResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserRatingsAsync(CancellationToken cancellationToken = default)
    {
        Guid? userId = HttpContext.GetUserId();

        IEnumerable<MovieRating> ratings = await _ratingService.GetRatingsForUserAsync(userId.Value, cancellationToken);

        IEnumerable<MovieRatingResponse> ratingsResponse = ratings.MapToResponse();
        
        return Ok(ratingsResponse);
    }
}*/