using ManyToMany.DAO.Entities;
using ManyToMany.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManyToMany.BLL
{
    public class MovieService
    {
        private readonly ApplicationDbContext context;
        private readonly ActorService actorService;
        public MovieService(ApplicationDbContext _context, ActorService _actorService)
        {
            context = _context;
            actorService = _actorService;
        }
        public async Task<IEnumerable<Movie>> ToListAsync()
        {
            return await context.Movies.ToListAsync();
        }
        public async Task<Movie> FindAsync(int movieId)
        {
            return await context.Movies
                .Where(a => a.Id == movieId)
                .Include(a => a.Actors)
                .FirstOrDefaultAsync(); ;
        }
        public async Task AddAsync(Movie movie)
        {
            context.Add(movie);
            await context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Movie movie)
        {
            context.Update(movie);
            await context.SaveChangesAsync();
        }

        public async Task AddActorAsync(int MovieId, int ActorId)
        {
            var movie = await FindAsync(MovieId);
            var actor = await actorService.FindAsync(ActorId);

            if (movie == null || actor == null)
            {
                throw new ArgumentNullException("O ator ou o filme não existem no banco de dados");
            }
            movie.Actors.Add(actor);
            await UpdateAsync(movie);
        }
        public async Task RemoveActorAsync(int MovieId, int ActorId)
        {
            var movie = await FindAsync(MovieId);
            var actor = await actorService.FindAsync(ActorId);

            if (movie == null || actor == null)
            {
                throw new ArgumentNullException("O ator ou o filme não existem no banco de dados");
            }
            movie.Actors.Remove(actor);
            await UpdateAsync(movie);
        }
        public async Task DeleteAsync(int movieId)
        {
            context.Movies.Remove(await FindAsync(movieId));
            await context.SaveChangesAsync();
        }
        public async Task<bool> AnyAsync(int id)
        {
            return await context.Movies.AnyAsync(e => e.Id == id);
        }
    }
}
