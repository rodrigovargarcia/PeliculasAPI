using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Entities
{
    public class Genero: IId
    {
        public int Id { get; set; }
        [Required]
        [StringLength(40)]
        public string Name { get; set; }
        public List<PeliculasGeneros> PeliculasGeneros { get; set; }
    }
}
