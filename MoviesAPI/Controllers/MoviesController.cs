using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Mapping;
using MoviesApplication.Models;
using MoviesApplication.Repositories;
using MoviesContracts.Requests;

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

        [HttpPost(ApiEndpoints.Movies.Create)]
        public async Task<IActionResult> Create(
            [FromBody] CreateMovieRequest request)
        {
            Movie movie = request.MapToMovie();

            await _movieRepository.CreateAsync(movie);

            // Note: we shouldn't be returning movie here, but rather should map movie to a new MovieResponse object and return that
            return Created($"{ApiEndpoints.Movies.Create}/{movie.Id}", movie);
        }
    }
}
