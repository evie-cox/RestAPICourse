using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Auth;
using MoviesAPI.Mapping;
using MoviesApplication.Models;
using MoviesApplication.Services;
using MoviesContracts.Requests;
using MoviesContracts.Responses;

namespace MoviesAPI.Controllers
{
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpPost(ApiEndpoints.Movies.Create, Name = nameof(Create))]
        public async Task<IActionResult> Create(
            [FromBody] CreateMovieRequest request,
            CancellationToken cancellationToken)
        {
            Movie movie = request.MapToMovie();

            await _movieService.CreateAsync(movie);

            return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie);
        }
        
        [HttpGet(ApiEndpoints.Movies.Get, Name = nameof(Get))]
        public async Task<IActionResult> Get(
            [FromRoute] string idOrSlug,
            CancellationToken cancellationToken)
        {
            Guid? userId = HttpContext.GetUserId();
            
            Movie? movie = Guid.TryParse(idOrSlug, out var id)
                ? await _movieService.GetByIdAsync(id, userId, cancellationToken)
                : await _movieService.GetBySlugAsync(idOrSlug, userId, cancellationToken);

            if (movie is null)
            {
                return NotFound();
            }

            MovieResponse response = movie.MapToResponse();
            return Ok(response);
        }
        
        [HttpGet(ApiEndpoints.Movies.GetAll, Name = nameof(GetAll))]
        public async Task<IActionResult> GetAll(
            [FromQuery] GetAllMoviesRequest request,
            CancellationToken cancellationToken)
        {
            Guid? userId = HttpContext.GetUserId();

            var options = request
                .MapToOptions()
                .WithUser(userId);
            
            IEnumerable<Movie> movies = await _movieService.GetAllAsync(options, cancellationToken);

            MoviesResponse moviesReponse = movies.MapToResponse();

            return Ok(moviesReponse);
        }

        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpPut(ApiEndpoints.Movies.Update, Name = nameof(Update))]
        public async Task<IActionResult> Update(
            [FromRoute] Guid id,
            [FromBody] UpdateMovieRequest request,
            CancellationToken cancellationToken)
        {
            Guid? userId = HttpContext.GetUserId();
            
            Movie movie = request.MapToMovie(id);

            Movie? updatedMovie = await _movieService.UpdateAsync(movie, userId, cancellationToken);

            if (updatedMovie is null)
            {
                return NotFound();
            }

            MovieResponse response = updatedMovie.MapToResponse();

            return Ok(response);
        }

        [Authorize(AuthConstants.AdminUserPolicyName)]
        [HttpDelete(ApiEndpoints.Movies.Delete, Name = nameof(Delete))]
        public async Task<IActionResult> Delete(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            bool deleted = await _movieService.DeleteByIdAsync(id, cancellationToken);

            if (!deleted)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
