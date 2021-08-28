using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ManyToMany.Data;
using ManyToMany.DAO.Entities;
using ManyToMany.BLL;
using System;

namespace ManyToMany.UI.Controllers
{
    public class MoviesController : Controller
    {
        private readonly MovieService movieService;
        private readonly ActorService actorService;

        public MoviesController(MovieService _movieService, ActorService _actorService)
        {
            movieService = _movieService;
            actorService = _actorService;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await movieService.ToListAsync());
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie movie)
        {
            if (ModelState.IsValid)
            {
                await movieService.AddAsync(movie);
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var movie = await movieService.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }

            // obtem lista de todos os atores
            var allActors = await actorService.ToListAsync();

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
        public async Task<IActionResult> Edit(int id, Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await movieService.UpdateAsync(movie);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await MovieExists(movie.Id))
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

        public async Task<IActionResult> AddActor(int MovieId, int ActorId)
        {
            try
            {
                await movieService.AddActorAsync(MovieId, ActorId);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
            }

            return RedirectToAction(nameof(Edit), new { id = MovieId });
        }
        public async Task<IActionResult> RemoveActor(int MovieId, int ActorId)
        {
            try
            {
                await movieService.RemoveActorAsync(MovieId, ActorId);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
            }
            return RedirectToAction(nameof(Edit), new { id = MovieId });
        }
        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await movieService.FindAsync(id);
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
            await movieService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> MovieExists(int id)
        {
            return await movieService.AnyAsync(id);
        }
    }
}
