using Microsoft.AspNetCore.Mvc;
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

        [HttpPost(ApiEndpoints.Movies.Create, Name = nameof(Create))]
        public async Task<IActionResult> Create(
            [FromBody] CreateMovieRequest request)
        {
            Movie movie = request.MapToMovie();

            await _movieService.CreateAsync(movie);

            return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie);
        }

        [HttpGet(ApiEndpoints.Movies.Get, Name = nameof(Get))]
        public async Task<IActionResult> Get(
            [FromRoute] string idOrSlug)
        {
            var movie = Guid.TryParse(idOrSlug, out var id)
                ? await _movieService.GetByIdAsync(id)
                : await _movieService.GetBySlugAsync(idOrSlug);

            if (movie is null)
            {
                return NotFound();
            }

            MovieResponse response = movie.MapToResponse();
            return Ok(response);
        }

        [HttpGet(ApiEndpoints.Movies.GetAll, Name = nameof(GetAll))]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Movie> movies = await _movieService.GetAllAsync();

            MoviesResponse moviesReponse = movies.MapToResponse();

            return Ok(moviesReponse);
        }

        [HttpPut(ApiEndpoints.Movies.Update, Name = nameof(Update))]
        public async Task<IActionResult> Update(
            [FromRoute] Guid id,
            [FromBody] UpdateMovieRequest request)
        {
            Movie movie = request.MapToMovie(id);

            Movie? updatedMovie = await _movieService.UpdateAsync(movie);

            if (updatedMovie is null)
            {
                return NotFound();
            }

            MovieResponse response = updatedMovie.MapToResponse();

            return Ok(response);
        }

        [HttpDelete(ApiEndpoints.Movies.Delete, Name = nameof(Delete))]
        public async Task<IActionResult> Delete(
            [FromRoute] Guid id)
        {
            var deleted = await _movieService.DeleteByIdAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
