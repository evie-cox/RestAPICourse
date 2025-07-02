using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using MoviesAPI.SDK;
using MoviesContracts.Requests;
using Refit;

var services = new ServiceCollection();
services.AddRefitClient<IMoviesApi>()
    .ConfigureHttpClient(x => 
        x.BaseAddress = new Uri("http://localhost:5236"));

var provider = services.BuildServiceProvider();

var moviesApi = provider.GetRequiredService<IMoviesApi>();

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
