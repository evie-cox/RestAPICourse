using Microsoft.Extensions.DependencyInjection;
using MoviesApplication.Database;
using MoviesApplication.Repositories;
using MoviesApplication.Repositories.Internal;
using MoviesApplication.Services;
using MoviesApplication.Services.Internal;

namespace MoviesApplication
{
    public static class ServiceCollectionsExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Singleton - instantiated only once and shared across the entire application for the lifetime of the application
            services.AddSingleton<IMovieRepository, MovieRepository>();
            services.AddSingleton<IMovieService, MovieService>();
            return services;
        }

        public static IServiceCollection AddDatabase(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddSingleton<IDBConnectionFactory>(_ =>
                new NpgsqlConnectionFactory(connectionString));
            services.AddSingleton<DBInitializer>();
            return services;
        }
    }
}
