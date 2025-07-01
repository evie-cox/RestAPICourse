using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MoviesAPI.Auth;
using MoviesAPI.Mapping;
using MoviesApplication.Models;
using MoviesApplication.Services;
using MoviesContracts.Requests;
using MoviesContracts.Responses;

namespace MoviesAPI.Controllers.V1
{
    [ApiController]
    [ApiVersion(1.0)]
    [ApiVersion(2.0)]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly IOutputCacheStore _outputCacheStore;

        public MoviesController(
            IMovieService movieService,
            IOutputCacheStore outputCacheStore)
        {
            _movieService = movieService;
            _outputCacheStore = outputCacheStore;
        }

        //[Authorize(AuthConstants.TrustedMemberPolicyName)]
        [ServiceFilter(typeof(ApiKeyAuthFilter))]
        [HttpPost(ApiEndpoints.Movies.Create, Name = nameof(Create))]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] CreateMovieRequest request,
            CancellationToken cancellationToken)
        {
            Movie movie = request.MapToMovie();

            await _movieService.CreateAsync(movie);

            await _outputCacheStore.EvictByTagAsync("movies", cancellationToken);
            
            return CreatedAtAction(nameof(Create), new { idOrSlug = movie.Id }, movie);
        }
        
        [MapToApiVersion(1.0)]
        [HttpGet(ApiEndpoints.Movies.Get, Name = nameof(GetV1))]
        [OutputCache(PolicyName = "MovieCache")]
        //[ResponseCache(Duration = 30, VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetV1(
            [FromRoute] string idOrSlug,
            [FromServices] LinkGenerator linkGenerator,
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
            
            var movieObj = new { id = movie.Id };
            response.Links.Add(new Link()
            {
                Href = linkGenerator.GetPathByRouteValues(HttpContext, nameof(GetV1), values: new { idOrSlug = movie.Id }),
                Rel = "self",
                Type = "GET"
            });
            
            response.Links.Add(new Link()
            {
                Href = linkGenerator.GetPathByRouteValues(HttpContext, nameof(Update), values: movieObj),
                Rel = "self",
                Type = "PUT"
            });
            
            response.Links.Add(new Link()
            {
                Href = linkGenerator.GetPathByRouteValues(HttpContext, nameof(Delete), values: movieObj),
                Rel = "self",
                Type = "DELETE"
            });
            
            return Ok(response);
        }
        
        [MapToApiVersion(2.0)]
        [HttpGet(ApiEndpoints.Movies.Get, Name = nameof(GetV2))]
        //[ResponseCache(Duration = 30, VaryByHeader = "Accept, Accept-Encoding", Location = ResponseCacheLocation.Any)]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetV2(
            [FromRoute] string idOrSlug,
            [FromServices] LinkGenerator linkGenerator,
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
        [OutputCache(PolicyName = "MovieCache")]
        //[ResponseCache(Duration = 30,
        //               VaryByQueryKeys = new[] { "title", "yearOfRelease", "sortBy", "pageNumber", "pageSize" },
        //               VaryByHeader = "Accept, Accept-Encoding",
        //               Location = ResponseCacheLocation.Any)]
        [ProducesResponseType(typeof(MoviesResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] GetAllMoviesRequest request,
            CancellationToken cancellationToken)
        {
            Guid? userId = HttpContext.GetUserId();

            var options = request
                .MapToOptions()
                .WithUser(userId);
            
            IEnumerable<Movie> movies = await _movieService.GetAllAsync(options, cancellationToken);
            
            int movieCount = await _movieService.GetCountAsync(options.Title, options.YearOfRelease, cancellationToken);

            MoviesResponse moviesReponse = movies.MapToResponse(request.PageNumber, request.PageSize, movieCount);

            return Ok(moviesReponse);
        }

        [Authorize(AuthConstants.TrustedMemberPolicyName)]
        [HttpPut(ApiEndpoints.Movies.Update, Name = nameof(Update))]
        [ProducesResponseType(typeof(MovieResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationFailureResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
            
            await _outputCacheStore.EvictByTagAsync("movies", cancellationToken);

            return Ok(response);
        }

        [Authorize(AuthConstants.AdminUserPolicyName)]
        [HttpDelete(ApiEndpoints.Movies.Delete, Name = nameof(Delete))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            [FromRoute] Guid id,
            CancellationToken cancellationToken)
        {
            bool deleted = await _movieService.DeleteByIdAsync(id, cancellationToken);

            if (!deleted)
            {
                return NotFound();
            }

            await _outputCacheStore.EvictByTagAsync("movies", cancellationToken);
            
            return Ok();
        }
    }
}
