using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Entities
{
    public class Actor: IId
    {
        public int Id { get; set; }
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        public DateTime BornDate { get; set; }
        public string Photo { get; set; }
        public List<PeliculasActores> PeliculasActores { get; set; }
    }
}
