using ManyToMany.DAO.Entities;
using ManyToMany.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManyToMany.BLL
{
    public class ActorService
    {
        private readonly ApplicationDbContext context;
        public ActorService(ApplicationDbContext _context)
        {
            context = _context;
        }
        public async Task<IEnumerable<Actor>> ToListAsync()
        {
            return await context.Actors.ToListAsync();
        }
        public async Task<Actor> FindAsync(int id)
        {
            return await context.Actors
                .Where(a => a.Id == id)
                .Include(a => a.Movies)
                .FirstOrDefaultAsync();
        }
        public async Task AddAsync(Actor actor)
        {
            context.Actors.Add(actor);
            await context.SaveChangesAsync();
        }
        public async Task AddMovieAsync(int ActorId, int MovieId)
        {
            var actor = await FindAsync(ActorId);

            var movie = context.Movies.Find(MovieId);

            if (actor == null || movie == null)
            {
                throw new ArgumentNullException("O ator ou o filme não existem no banco de dados");
            }

            actor.Movies.Add(movie);
            await UpdateAsync(actor);
        }
        public async Task UpdateAsync(Actor actor)
        {
            context.Actors.Update(actor);
            await context.SaveChangesAsync();
        }
        public async Task RemoveMovieAsync(int ActorId, int MovieId)
        {
            var actor = await FindAsync(ActorId);
            var movie = context.Movies.Find(MovieId);

            if (actor == null || movie == null)
            {
                throw new ArgumentNullException("O ator ou o filme não existem no banco de dados");
            }

            actor.Movies.Remove(movie);
            context.Actors.Update(actor);
            context.SaveChanges();
        }
        public async Task DeleteAsync(int actorId)
        {
            context.Actors.Remove(await FindAsync(actorId));
            await context.SaveChangesAsync();
        }
        public async Task<bool> AnyAsync(int actorId)
        {
            return await context.Actors.AnyAsync(a => a.Id == actorId);
        }
    }
}
