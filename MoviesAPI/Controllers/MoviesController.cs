using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using MoviesAPI.Models;
using System.Linq.Expressions;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public MoviesController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;

        }
        [HttpGet(Name = "Movies")]
        public async Task<ActionResult<IEnumerable<Movie>>> Movies([FromQuery] string? searchTerm, string? sortColumn, string? sortOrder, int pageNumber, int pageSize)
        {

            // Filtering
            IQueryable<Movie> moviesQuery = _dbContext.Movies;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                moviesQuery = moviesQuery.Where(m => m.Title.Contains(searchTerm) || m.Director.Contains(searchTerm));
            }

            Expression<Func<Movie, object>> keySelector = sortColumn?.ToLower() switch
            {
                "title" => movie => movie.Title,
                "director" => movie => movie.Director,
                "rating" => movie => movie.Rating,
                _ => movie => movie.MovieID
            };

            // Sorting
            if (sortOrder?.ToLower() == "desc")
            {
                moviesQuery = moviesQuery.OrderByDescending(keySelector);
            }
            else
            {
                moviesQuery = moviesQuery.OrderBy(keySelector);
            }

            // Projection
            var moviesResponsesQuery = moviesQuery.Select(m => new MovieResponse
            {
                Title = m.Title,
                Description = m.Description,
                Rating = m.Rating,
                Director = m.Director,
                ReleaseDate = m.ReleaseDate,
            });

            // Paging at database level
            var products = await PagedList<MovieResponse>.CreateAsync(moviesResponsesQuery, pageNumber, pageSize);
            return Ok(products);
            //var movies = await moviesQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(m => new MovieResponse
            //{
            //    Title = m.Title,
            //    Description = m.Description,
            //    Rating = m.Rating,
            //    Director = m.Director,
            //    ReleaseDate = m.ReleaseDate,
            //}).ToListAsync();
            // return Ok(movies);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> Movies(int id)
        {
            if (id == 0)
            {
                return BadRequest("ID cannot be a value of 0");
            }
            var movie = await _dbContext.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return Ok(movie);
        }
        [HttpPost]
        public async Task<IActionResult> Movies([FromBody] Movie movie)
        {
            if (movie == null)
            {
                return BadRequest();
            }
            _dbContext.Movies.Add(movie);
            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(Movies), new { id = movie.MovieID }, movie);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMovie(int id, Movie movie)
        {
            if (id != movie.MovieID)
            {
                return BadRequest();
            }
            _dbContext.Entry(movie).State = EntityState.Modified;
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                    return NotFound();
                throw;
            }
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovies(int id)
        {
            var movie = await _dbContext.Movies.FindAsync(id);
            if (movie == null)
                return NotFound();
            _dbContext.Movies.Remove(movie);
            await _dbContext.SaveChangesAsync();

            return NoContent();
        }



        private bool MovieExists(int id)
        {
            return _dbContext.Movies.Any(e => e.MovieID == id);
        }
    }
}
