using MoviesApplication.Models;
using MoviesContracts.Requests;

namespace MoviesAPI.Mapping
{
    public static class ContractMapping
    {
        public static Movie MapToMovie(this CreateMovieRequest request)
        {
            return new Movie
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                YearOfRelease = request.YearOfRelease,
                Genres = request.Genres.ToList(),
            };
        }
    }
}
