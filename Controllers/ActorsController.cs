using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExercicioEfManyToMany.Data;
using ExercicioEfManyToMany.Models;

namespace ExercicioEfManyToMany
{
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Actors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Actors.ToListAsync());
        }

        // GET: Actors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // GET: Actors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Actors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Actor actor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // só é possivel usar o 'Include' após a clausula 'Where'
            var actor = await _context.Actors
                .Where(a => a.Id == id)
                .Include(a => a.Movies)
                .FirstOrDefaultAsync();

            if (actor == null)
            {
                return NotFound();
            }

            // obtenho uma lista de todos os filmes
            var allMovies = _context.Movies.ToList();

            // obtenho a lista de filmes que o ator já está relacionado
            var moviesAdded = actor.Movies;

            // obtenho uma lista de filmes que o ator NÃO está adicionado
            var moviesNotAdd = allMovies.Except(moviesAdded).ToList();

            ActorMoviesViewModel viewModel = new ActorMoviesViewModel()
            {
                Actor = actor,
                MoviesNotAdded = moviesNotAdd,
            };
            return View(viewModel);
        }

        // POST: Actors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Actor actor)
        {
            if (id != actor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(actor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorExists(actor.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult AddMovie(int ActorId, int MovieId)
        {
            var actor = _context.Actors
                .Where(a => a.Id == ActorId)
                .Include(a => a.Movies)
                .FirstOrDefault();
            var movie = _context.Movies.Find(MovieId);

            if (actor == null || movie == null)
            {
                return NotFound();
            }

            actor.Movies.Add(movie);
            _context.Actors.Update(actor);
            _context.SaveChanges();

            return RedirectToAction(nameof(Edit), new { id = ActorId });
        }

        [HttpPost]
        public IActionResult RemoveMovie(int ActorId, int MovieId)
        {
            var actor = _context.Actors
                .Where(a => a.Id == ActorId)
                .Include(a => a.Movies)
                .FirstOrDefault();
            var movie = _context.Movies.Find(MovieId);

            if (actor == null || movie == null)
            {
                return NotFound();
            }

            actor.Movies.Remove(movie);
            _context.SaveChanges();

            return RedirectToAction(nameof(Edit), new { id = ActorId });
        }

        // GET: Actors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actor = await _context.Actors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // POST: Actors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actor = await _context.Actors.FindAsync(id);
            _context.Actors.Remove(actor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActorExists(int id)
        {
            return _context.Actors.Any(e => e.Id == id);
        }
    }
}
