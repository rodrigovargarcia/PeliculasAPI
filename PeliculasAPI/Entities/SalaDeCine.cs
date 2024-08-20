using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Entities
{
    public class SalaDeCine: IId
    {
        public int Id { get; set; }
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        public Point Ubication { get; set; }    
        public List<PeliculasSalasDeCine> PeliculasSalasDeCine { get; set; }
    }
}
