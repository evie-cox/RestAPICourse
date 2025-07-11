﻿using MoviesApplication.Models;
using MoviesContracts.Requests;
using MoviesContracts.Responses;

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

        public static Movie MapToMovie(this UpdateMovieRequest request, Guid id)
        {
            return new Movie
            {
                Id = id,
                Title = request.Title,
                YearOfRelease = request.YearOfRelease,
                Genres = request.Genres.ToList(),
            };
        }

        public static MovieResponse MapToResponse(this Movie movie)
        {
            return new MovieResponse
            {
                Id = movie.Id,
                Title = movie.Title,
                Slug = movie.Slug,
                Rating = movie.Rating,
                UserRating = movie.UserRating,
                YearOfRelease = movie.YearOfRelease,
                Genres = movie.Genres.ToList(),
            };
        }

        public static MoviesResponse MapToResponse(
            this IEnumerable<Movie> movies,
            int pageNumber,
            int pageSize,
            int totalCount)
        {
            return new MoviesResponse
            {
                Movies = movies.Select(MapToResponse),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public static IEnumerable<MovieRatingResponse> MapToResponse(this IEnumerable<MovieRating> movieRatings)
        {
            return movieRatings.Select(x => new MovieRatingResponse
            {
                Rating = x.Rating,
                Slug = x.Slug,
                MovieId = x.MovieId,
            });
        }

        public static GetAllMoviesOptions MapToOptions(this GetAllMoviesRequest request)
        {
            return new GetAllMoviesOptions
            {
                Title = request.Title,
                YearOfRelease = request.YearOfRelease,
                SortField = request.SortBy?.Trim('+', '-'),
                SortOrder = request.SortBy is null ? SortOrder.Unsorted
                    : request.SortBy.StartsWith("-") ? SortOrder.Descending : SortOrder.Ascending,
                PageNumber = request.PageNumber.GetValueOrDefault(PagedRequest.DefaultPageNumber),
                PageSize = request.PageSize.GetValueOrDefault(PagedRequest.DefaultPageSize),
            };
        }

        public static GetAllMoviesOptions WithUser(this GetAllMoviesOptions options, Guid? userId)
        {
            options.UserId = userId;
            
            return options;
        }
    }
}
