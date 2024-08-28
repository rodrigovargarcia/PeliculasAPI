using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entities;

namespace PeliculasAPI.Tests.PruebasDeIntegracion
{
    [TestClass]
    public class GenerosControllerTests: BasePruebas
    {
        private static string url = "/api/generos";

        [TestMethod]
        public async Task ObtenerTodosLosGenerosListadoVacio()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDB);

            var cliente = factory.CreateClient();
            var respuesta = await cliente.GetAsync(url);

            respuesta.EnsureSuccessStatusCode();

            var generos = JsonConvert.DeserializeObject<List<GeneroDTO>>(await respuesta.Content.ReadAsStringAsync());

            Assert.AreEqual(0, generos.Count);
        }

        [TestMethod]
        public async Task ObtenerTodosLosGeneros()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDB);

            var context = ConstruirContext(nombreDB);

            context.Generos.Add(new Genero() { Name = "Genero1" });
            context.Generos.Add(new Genero() { Name = "Genero2" });
            await context.SaveChangesAsync();

            var cliente = factory.CreateClient();
            var respuesta = await cliente.GetAsync(url);

            respuesta.EnsureSuccessStatusCode();

            var generos = JsonConvert.DeserializeObject<List<GeneroDTO>>(await respuesta.Content.ReadAsStringAsync());

            Assert.AreEqual(2, generos.Count);
        }

        [TestMethod]
        public async Task BorrarGenero()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDB);

            var context = ConstruirContext(nombreDB);
            context.Generos.Add(new Genero() { Name = "Genero1" });
            await context.SaveChangesAsync();

            var cliente = factory.CreateClient();
            var respuesta = await cliente.DeleteAsync($"{url}/1");

            respuesta.EnsureSuccessStatusCode();

            var context2 = ConstruirContext(nombreDB);
            var existe = await context2.Generos.AnyAsync();
            Assert.IsFalse(existe);
        }

        [TestMethod]
        public async Task BorrarGeneroRetorna401()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDB, ignorarSeguridad: false);

            var cliente = factory.CreateClient();
            var respuesta = await cliente.DeleteAsync($"{url}/1");

            Assert.AreEqual("Unauthorized", respuesta.ReasonPhrase);
        }
    }
}
