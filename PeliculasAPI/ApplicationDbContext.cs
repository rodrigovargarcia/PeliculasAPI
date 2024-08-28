using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using PeliculasAPI.Entities;
using System.Security.Claims;

namespace PeliculasAPI
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PeliculasActores>()
                .HasKey(x => new { x.ActorId, x.PeliculaId });

            modelBuilder.Entity<PeliculasGeneros>()
                .HasKey(x => new { x.GeneroId, x.PeliculaId });

            modelBuilder.Entity<PeliculasSalasDeCine>()
                .HasKey(x => new { x.PeliculaId, x.SalaDeCineId });

            SeedData(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            //var rolAdminId = "2ead7cb2-4cde-4944-b58e-0df867c043d1";
            //var userAdminId = "b400e8d2-2134-4a3f-beda-2316bfcbad49";

            //var rolAdmin = new IdentityRole()
            //{
            //    Id = rolAdminId,
            //    Name = "Admin",
            //    NormalizedName = "Admin"
            //};

            //var passwordHasher = new PasswordHasher<IdentityUser>();

            //var userName = "rodrigo@admin.com";

            //var userAdmin = new IdentityUser()
            //{
            //    Id = userAdminId,
            //    UserName = userName,
            //    NormalizedUserName = userName,
            //    Email = userName,
            //    NormalizedEmail = userName,
            //    PasswordHash = passwordHasher.HashPassword(null, "Aa123456!")
            //};

            //modelBuilder.Entity<IdentityUser>()
            //    .HasData(userAdmin);

            //modelBuilder.Entity<IdentityRole>()
            //    .HasData(rolAdmin);

            //modelBuilder.Entity<IdentityUserClaim<string>>()
            //    .HasData(new IdentityUserClaim<string>()
            //    {
            //        Id = 1,
            //        ClaimType = ClaimTypes.Role,
            //        UserId = userAdminId,
            //        ClaimValue = "Admin"
            //    });

            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            modelBuilder.Entity<SalaDeCine>()
                .HasData(new List<SalaDeCine>
                {
                    new SalaDeCine {Id = 4, Name = "Cine Renzi", Ubication = geometryFactory.CreatePoint(new Coordinate(-64.24286,-27.73181))},
                    new SalaDeCine {Id = 5, Name = "Cine No se ve", Ubication = geometryFactory.CreatePoint(new Coordinate(-64.26008,-27.79331))},
                    new SalaDeCine {Id = 6, Name = "Cine Hoytz", Ubication = geometryFactory.CreatePoint(new Coordinate(-64.18833,-31.42011))}
                });
        }

        public DbSet<Genero> Generos { get; set; }

        public DbSet<Actor> Actores { get; set; }

        public DbSet<Pelicula> Peliculas { get; set; }

        public DbSet<PeliculasActores> PeliculasActores { get; set; }

        public DbSet<PeliculasGeneros> PeliculasGeneros { get; set; }
        public DbSet<SalaDeCine> SalasDeCine { get; set; }
        public DbSet<PeliculasSalasDeCine> PeliculasSalasDeCine { get; set; }
        public DbSet<Review> Reviews { get; set; }
    }
}
