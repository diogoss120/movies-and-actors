using ManyToMany.DAO.Entities;
using Microsoft.EntityFrameworkCore;

namespace ManyToMany.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

       public DbSet<Movie> Movies {get; set;}
       public DbSet<Actor> Actors {get; set;}
    }
}