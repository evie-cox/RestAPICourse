using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using MoviesAPI.SDK;
using MoviesAPI.SDK.Consumer;
using MoviesContracts.Requests;
using Refit;

// Attempted to introduce handling token generation and refreshing, but I keep getting a CS1593 delegate error on the AddRefitClient
// Something I need to come back to later on
/*var services = new ServiceCollection();

services
    .AddHttpClient()
    .AddSingleton<AuthTokenProvider>()
    .AddRefitClient<IMoviesApi>(s => new RefitSettings
    { 
        AuthorizationHeaderValueGetter = async () => await s.GetRequiredService<AuthTokenProvider>().GetTokenAsync()
    })
    .ConfigureHttpClient(x => 
        x.BaseAddress = new Uri("http://localhost:5236"));

var provider = services.BuildServiceProvider();
var moviesApi = provider.GetRequiredService<IMoviesApi>();*/

var moviesApi = RestService.For<IMoviesApi>("https://localhost:44375");

var movie = await moviesApi.GetMovieAsync("ponyo-2008");

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
