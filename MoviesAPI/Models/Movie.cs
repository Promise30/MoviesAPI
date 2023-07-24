using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Models
{
    public class Movie
    {
        public int MovieID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Director { get; set; }
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }
        [Range(0.0, 10.0, ErrorMessage = "Rating must be between 0.0 and 10.0")]
        public decimal Rating { get; set; }

    }
}
