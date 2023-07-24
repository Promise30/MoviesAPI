using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using MoviesAPI.Models;

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
        public async Task<ActionResult<List<Movie>>> Movies()
        {
            var movies = await _dbContext.Movies.ToListAsync();
            if (movies == null)
                return NotFound();
            return Ok(movies);
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
