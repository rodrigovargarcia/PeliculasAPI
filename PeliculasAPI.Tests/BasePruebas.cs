using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using PeliculasAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasAPI.Tests
{
    public class BasePruebas
    {
        protected string usuarioPorDefectoId = "9722b56a-77ea-4e41-941d-e319b6eb3712";
        protected string usuarioPorDefectoEmail = "ejemplo@hotmail.com";
        protected ApplicationDbContext ConstruirContext(string nombreDB)
        {
            var opciones = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(nombreDB).Options;

            var dbContext = new ApplicationDbContext(opciones);
            return dbContext;
        }

        protected IMapper ConfigurarAutoMapper()
        {
            var config = new MapperConfiguration(options =>
            {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                options.AddProfile(new AutoMapperProfiles(geometryFactory));
            });

            return config.CreateMapper();
        }

        protected ControllerContext ConstruirControllerContext()
        {
            var usuario = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]        // técnicamente, es un objeto complejo. Inicializamos un objeto con su constructor combinando la sintaxis con una inicialización de colecciones
            {
                new Claim(ClaimTypes.Name, usuarioPorDefectoEmail),
                new Claim(ClaimTypes.Email, usuarioPorDefectoEmail),
                new Claim(ClaimTypes.NameIdentifier, usuarioPorDefectoId)
            }));

            return new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = usuario }
            };

        }
        // Esta clase WebApplicationFactory es propia de ASP .NET Core y está diseñada para ayudarnos a realizar pruebas de integración de aplicaciones web.
        protected WebApplicationFactory<Startup> ConstruirWebApplicationFactory(string nombreDB, bool ignorarSeguridad = true)  // la variable de tipo bool ignorarSeguridad la utilizamos para ignorar los Authorize de nuestros endpoints. 
                                                                                                                                // Permite crear una instancia de nuestra app web completa en un entorno de pruebas.
        {
            // Crea una instancia de WebApplicationFactory para la aplicación especificada <Startup>.
            // WebApplicationFactory se usa para configurar y construir una instancia de la aplicación para pruebas.
            var factory = new WebApplicationFactory<Startup>();

            factory = factory.WithWebHostBuilder(builder =>     // Configura la instancia de WebApplicationFactory usando un WebHostBuilder. Lo que nos permite personalizar el entorno de pruebas
            {
                builder.ConfigureTestServices(services =>       // Desde aquí podemos configurar nuestro sistema de inyección de dependencias.
                {
                    var descriptorDBContext = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));   // Aquí básicamente lo que hacemos el buscar el servicio que nos provee nuestro DB Context. 
                                                                                        // Como queremos realizar pruebas desde un entorno de pruebas en memoria, debemos de remover el servicio que nos provee el ApplicationDbContext
                    if(descriptorDBContext != null)     // Si se encuentra un descriptorDbContext de tipo ApplicationDbContext
                    {
                        services.Remove(descriptorDBContext);   // Aquí lo removemos.
                    }

                    // Registra un DbContext en memoria para pruebas especificado por la variable nombreDB, en lugar de la base de datos real
                    services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase(nombreDB));     // y luego utilizamos DB en memoria directamente

                    if (ignorarSeguridad)   // para ignorar los Authorize, utilizamos estas 2 clases que creamos específicamente para esto.
                    {
                        services.AddSingleton<IAuthorizationHandler, AllowAnonimousHandler>();  // Registra un Servicio de tipo IAuthorizationHandler que permite el acceso anónimo a los recursos gracias a nuestra clase AllowAnonimousHandler

                        services.AddControllers(options =>
                        {
                            options.Filters.Add(new UsuarioFalsoFiltro());  // Agrega un Filtro que hereda de IAsyncActionFilter (corre antes y después de una acción), que nos permite tener acceso a Claims que soliciten información del usuario.
                        });
                    }
                });
            });
            return factory;
        }
    }
}
