namespace AsyncDemoWebAppComplete.Models
{
    public class Movie
    {
        public string Title { get; set; }
        public int Year { get; set; }
        public string ImageUrl { get; set; }
        public string Category { get; set; }
    }

    public enum Genre { Action, Drama, Thriller, SciFi }
}