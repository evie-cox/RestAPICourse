using Microsoft.Extensions.DependencyInjection;
using MoviesApplication.Repositories;
using MoviesApplication.Repositories.Internal;

namespace MoviesApplication
{
    public static class ServiceCollectionsExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            /// <remarks>
            /// Singleton - instantiated only once and shared across the entire application for the lifetime of the application
            /// </remarks>
            services.AddSingleton<IMovieRepository, MovieRepository>();
            return services;
        }
    }
}
