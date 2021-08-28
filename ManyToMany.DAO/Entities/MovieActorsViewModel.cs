using System.Collections.Generic;

namespace ManyToMany.DAO.Entities
{
    public class MovieActorsViewModel
    {
        public Movie Movie { get; set; }
        public List<Actor> ActorsNotAdded { get; set; }

        public MovieActorsViewModel(){
            ActorsNotAdded = new List<Actor>();
        }
    }
}