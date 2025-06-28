using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Mapping;
using MoviesApplication.Models;
using MoviesApplication.Repositories;
using MoviesContracts.Requests;
using MoviesContracts.Responses;

namespace MoviesAPI.Controllers
{
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieRepository _movieRepository;

        public MoviesController(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        [HttpPost(ApiEndpoints.Movies.Create, Name = nameof(Create))]
        public async Task<IActionResult> Create(
            [FromBody] CreateMovieRequest request)
        {
            Movie movie = request.MapToMovie();

            await _movieRepository.CreateAsync(movie);

            return CreatedAtAction(nameof(Get), new { id = movie.Id }, movie);
        }

        [HttpGet(ApiEndpoints.Movies.Get, Name = nameof(Get))]
        public async Task<IActionResult> Get(
            [FromRoute] Guid id)
        {
            var movie = await _movieRepository.GetByIdAsync(id);

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
            IEnumerable<Movie> movies = await _movieRepository.GetAllAsync();

            MoviesResponse moviesReponse = movies.MapToResponse();

            return Ok(moviesReponse);
        }

        [HttpPut(ApiEndpoints.Movies.Update, Name = nameof(Update))]
        public async Task<IActionResult> Update(
            [FromRoute] Guid id,
            [FromBody] UpdateMovieRequest request)
        {
            Movie movie = request.MapToMovie(id);

            bool updated = await _movieRepository.UpdateAsync(movie);

            if (!updated)
            {
                return NotFound();
            }

            MovieResponse response = movie.MapToResponse();

            return Ok(response);
        }

        [HttpDelete(ApiEndpoints.Movies.Delete, Name = nameof(Delete))]
        public async Task<IActionResult> Delete(
            [FromRoute] Guid id)
        {
            var deleted = await _movieRepository.DeleteByIdAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return Ok();
        }
    }
}
