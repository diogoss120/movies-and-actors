using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManyToMany.DAO.Entities
{
    public class ActorMoviesViewModel
    {
        public Actor Actor { get; set; }
        public List<Movie> MoviesNotAdded { get; set; }

        public ActorMoviesViewModel()
        {
            MoviesNotAdded = new List<Movie>();
        }
    }
}
