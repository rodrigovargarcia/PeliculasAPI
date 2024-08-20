namespace PeliculasAPI.Entities
{
    public class PeliculasSalasDeCine
    {
        public int PeliculaId { get; set; }
        public int SalaDeCineId { get; set; }
        public SalaDeCine SalaDeCine { get; set; }
        public Pelicula Peliculas { get; set; }
    }
}
