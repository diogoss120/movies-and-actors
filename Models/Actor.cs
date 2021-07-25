using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExercicioEfManyToMany.Models
{
    public class Actor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "informe o nome do ator")]
        public string Name { get; set; }
        public List<Movie> Movies { get; set; }

        public Actor()
        {
            Movies = new List<Movie>();
        }
    }
}