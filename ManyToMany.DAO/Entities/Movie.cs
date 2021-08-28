using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ManyToMany.DAO.Entities
{
    public class Movie
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Informe o nome do filme")]
        public string Name { get; set; }
        public List<Actor> Actors { get; set; }

        public Movie()
        {
            Actors = new List<Actor>();
        }
    }
}