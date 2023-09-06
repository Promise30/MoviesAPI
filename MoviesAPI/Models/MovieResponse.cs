using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesAPI.Models
{
    public class MovieResponse
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Director { get; set; }
        [Column(TypeName = "date")]
        public DateTime ReleaseDate { get; set; }
        [Range(0.0, 10.0, ErrorMessage = "Rating must be between 0.0 and 10.0")]
        public decimal Rating { get; set; }
    }
}
