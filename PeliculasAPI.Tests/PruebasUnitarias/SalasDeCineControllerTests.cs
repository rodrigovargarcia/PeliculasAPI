using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using PeliculasAPI.Controllers;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasAPI.Tests.PruebasUnitarias
{
    [TestClass]
    public class SalasDeCineControllerTests: BasePruebas
    {
        [TestMethod]
        public async Task ObtenerSalasDeCineA5KmOMenos()
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

            using (var context = LocalDbDatabaseInitializer.GetDbContextLocalDb(false))
            {
                var salasDeCine = new List<SalaDeCine>()
                {
                    new SalaDeCine{Name = "Cine Sunstar", Ubication = geometryFactory.CreatePoint(new Coordinate(-64.257520, -27.776851))}
                };

                context.AddRange(salasDeCine);
                await context.SaveChangesAsync();
            }

            var filtro = new SalaDeCineCercanoFiltroDTO()
            {
                DistanciaEnKms = 5,
                Latitud = -27.779837,
                Longitud = -64.258095
            };

            using (var context = LocalDbDatabaseInitializer.GetDbContextLocalDb(false))
            {
                var mapper = ConfigurarAutoMapper();
                var controller = new SalaDeCineController(context, mapper, geometryFactory);
                var respuesta = await controller.Cercanos(filtro);
                var valor = respuesta.Value;
                Assert.AreEqual(2, valor.Count);
            }
        }
    }
}
