using System.Text.RegularExpressions;

namespace MoviesApplication.Models
{
    public class Movie
    {
        public required Guid Id { get; init; }

        public required string Title { get; set; }

        public string Slug => GenerateSlug();

        public required int YearOfRelease { get; set; }
        
        public int? UserRating { get; set; }
        
        public float? Rating { get; set; }

        public required List<string> Genres { get; init; } = new();

        private string GenerateSlug()
        {
            string sluggedTitle = Regex
                .Replace(Title, "[^0-9A-Za-z _-]", string.Empty)
                .ToLower()
                .Replace(" ", "-");

            return $"{sluggedTitle}-{YearOfRelease}";
        }
    }
}
