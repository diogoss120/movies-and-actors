using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExercicioEfManyToMany.Data;
using ExercicioEfManyToMany.Models;

namespace ExercicioEfManyToMany
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movies.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Where(m => m.Id == id)
                .Include(m => m.Actors)
                .SingleOrDefaultAsync();

            if (movie == null)
            {
                return NotFound();
            }

            // obtem lista de todos os atores
            var allActors = _context.Actors.ToList();

            // obtem uma lista de todos os atores que participam desse filme
            var actorsAdded = movie.Actors;

            // obtem uma lista de todos os atores que não participam do filme
            var actorsNotAdded = allActors.Except(actorsAdded).ToList();

            MovieActorsViewModel viewModel = new MovieActorsViewModel
            {
                Movie = movie,
                ActorsNotAdded = actorsNotAdded,
            };

            return View(viewModel);
        }

        // POST: Movies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        public IActionResult AddActor(int MovieId, int ActorId)
        {
            var movie = _context.Movies
                .Where(m => m.Id == MovieId)
                .Include(m => m.Actors)
                .SingleOrDefault();
            var actor = _context.Actors.Find(ActorId);

            if (movie == null || actor == null)
            {
                return NotFound();
            }
            movie.Actors.Add(actor);
            _context.Movies.Update(movie);
            _context.SaveChanges();

            return RedirectToAction(nameof(Edit), new { id = MovieId });
        }
        public IActionResult RemoveActor(int MovieId, int ActorId)
        {
            var movie = _context.Movies
                .Where(m => m.Id == MovieId)
                .Include(m => m.Actors)
                .SingleOrDefault();
            var actor = _context.Actors.Find(ActorId);

            if (movie == null || actor == null)
            {
                return NotFound();
            }
            movie.Actors.Remove(actor);
            _context.Movies.Update(movie);
            _context.SaveChanges();

            return RedirectToAction(nameof(Edit), new { id = MovieId });
        }
        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.FindAsync(id);
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}
