using System.Text.Json;
using MoviesAPI.SDK;
using MoviesContracts.Requests;
using Refit;

var moviesApi = RestService.For<IMoviesApi>("https://localhost:44375");

// var movie = await moviesApi.GetMovieAsync("about-a-boy-2002");
// Console.WriteLine(JsonSerializer.Serialize(movie));

var request = new GetAllMoviesRequest
{
    Title = null,
    YearOfRelease = null,
    SortBy = null,
    PageNumber = 1,
    PageSize = 5,
};

var movies = await moviesApi.GetAllMoviesAsync(request);

foreach (var movieResponse in movies.Movies)
{
    Console.WriteLine(JsonSerializer.Serialize(movieResponse));
}
