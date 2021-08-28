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
    public class ActorsController : Controller
    {
        private readonly MovieService movieService;
        private readonly ActorService actorService;

        public ActorsController(MovieService _movieService, ActorService _actorService)
        {
            movieService = _movieService;
            actorService = _actorService;
        }

        // GET: Actors
        public async Task<IActionResult> Index()
        {
            return View(await actorService.ToListAsync());
        }

        // GET: Actors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Actors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Actor actor)
        {
            if (ModelState.IsValid)
            {
                await actorService.AddAsync(actor);
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actors/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            // só é possivel usar o 'Include' após a clausula 'Where'
            var actor = await actorService.FindAsync(id);

            if (actor == null)
            {
                return NotFound();
            }

            // obtenho uma lista de todos os filmes
            var allMovies = await movieService.ToListAsync();

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
        public async Task<IActionResult> Edit(int id, Actor actor)
        {
            if (id != actor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await actorService.UpdateAsync(actor);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ActorExists(actor.Id))
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

        public async Task<IActionResult> AddMovie(int ActorId, int MovieId)
        {
            try
            {
                await actorService.AddMovieAsync(ActorId, MovieId);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
            }

            return RedirectToAction(nameof(Edit), new { id = ActorId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveMovie(int ActorId, int MovieId)
        {
            try
            {
                await actorService.RemoveMovieAsync(ActorId, MovieId);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
            }
            return RedirectToAction(nameof(Edit), new { id = ActorId });
        }

        // GET: Actors/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var actor = await actorService.FindAsync(id);

            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // POST: Actors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int actorId)
        {
            await actorService.DeleteAsync(actorId);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ActorExists(int actorId)
        {
            return await actorService.AnyAsync(actorId);
        }
    }
}
