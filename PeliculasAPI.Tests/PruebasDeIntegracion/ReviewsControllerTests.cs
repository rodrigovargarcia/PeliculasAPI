using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PeliculasAPI.Entities;

namespace PeliculasAPI.Tests.PruebasDeIntegracion
{
    [TestClass]
    public class ReviewsControllerTests: BasePruebas
    {
        private static readonly string url = "/api/peliculas/1/reviews";

        [TestMethod]
        public async Task UsuarioObtieneUn404NoExistePelicula()     // Al no existir película creada con el Id = 1, devuelve 404. Para entender mejor el contexto, revisar el ReviewController.cs
        {            
            var nombreDB = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDB);

            var cliente = factory.CreateClient();
            var respuesta = await cliente.GetAsync(url);

            Assert.AreEqual(404, (int)respuesta.StatusCode);     // Hacemos un casteo explícito para respuesta.StatusCode
        }

        [TestMethod]
        public async Task DevuelveListadoVacio()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDB);


            var context = ConstruirContext(nombreDB);
            context.Peliculas.Add(new Pelicula() { Titulo = "Pelicula1" });
            await context.SaveChangesAsync();

            var cliente = factory.CreateClient();
            var respuesta = await cliente.GetAsync(url);
            respuesta.EnsureSuccessStatusCode();

            var reviews = JsonConvert.DeserializeObject<List<Review>>(await respuesta.Content.ReadAsStringAsync());     //Deserializamos hacia un List<ReviewDTO> para poder leer el listado vacío de Reviews.
                                                                                                                        //Realizamos esta DESERIALIZACIÓN debido a que el servidor nos responde con datos en
                                                                                                                        //formato JSON, por lo tanto, los deserializamos a un formato List<Reviews>
            Assert.AreEqual(0, reviews.Count);
        }
    }
}
