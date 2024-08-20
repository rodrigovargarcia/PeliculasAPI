﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeliculasAPI.Controllers;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasAPI.Tests.PruebasUnitarias
{
    [TestClass]
    public class GenerosControllerTests: BasePruebas
    {
        [TestMethod]
        public async Task ObtenerTodosLosGeneros()
        {
            // PARTES DE UNA PRUEBA

            // PREPARACIÓN:
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();


            contexto.Generos.Add(new Genero() { Name = "Genero 1" });
            contexto.Generos.Add(new Genero() { Name = "Genero 2" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreDB);


            // PRUEBA:
            var controller = new GeneroController(contexto2, mapper);
            var respuesta = await controller.Get();

            // RESULTADO - VERIFICACIÓN:

            var generos = respuesta.Value;

            Assert.AreEqual(2, generos.Count);
        }

        [TestMethod]
        public async Task ObtenerGeneroPorIdNoExistente()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            var controller = new GeneroController(contexto, mapper);
            var respuesta = await controller.Get(1);

            var resultado = respuesta.Result as StatusCodeResult;
            Assert.AreEqual(404, resultado.StatusCode);
        }
        [TestMethod]
        public async Task ObtenerGeneroPorIdExistente()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();


            contexto.Generos.Add(new Genero() { Name = "Genero 1" });
            contexto.Generos.Add(new Genero() { Name = "Genero 2" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreDB);
            var controller = new GeneroController(contexto2, mapper);

            var id = 1;

            var respuesta = await controller.Get(id);

            var resultado = respuesta.Value;
            Assert.AreEqual(id, resultado.Id);
        }

        [TestMethod]
        public async Task CrearGenero()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            var nuevoGenero = new GeneroCreacionDTO() { Name = "nuevo género" };

            var controller = new GeneroController(contexto, mapper);

            var respuesta = await controller.Post(nuevoGenero);

            var resultado = respuesta as CreatedAtRouteResult;

            Assert.IsNotNull(resultado);

            var contexto2 = ConstruirContext(nombreDB);
            var cantidad = await contexto2.Generos.CountAsync();
            Assert.AreEqual(1, cantidad);
        }

        [TestMethod]
        public async Task ActualizarGenero()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            contexto.Generos.Add(new Genero(){ Name = "Género 1" });
            await contexto.SaveChangesAsync();


            var contexto2 = ConstruirContext(nombreDB);
            var controller = new GeneroController(contexto2, mapper);

            var generoCreacionDTO = new GeneroCreacionDTO() { Name = "Nuevo Género" };

            var id = 1;
            var respuesta = await controller.Put(id, generoCreacionDTO);

            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado.StatusCode);

            var contexto3 = ConstruirContext(nombreDB);
            var existe = await contexto3.Generos.AnyAsync(x => x.Name == "Nuevo Género");
            Assert.IsTrue(existe);
        }

        [TestMethod]
        public async Task IntentaBorrarUnGeneroNoExistente()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            var controller = new GeneroController(contexto, mapper);
            var respuesta = await controller.Delete(1);
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(404, resultado.StatusCode);
        }

        [TestMethod]
        public async Task BorrarGenero()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreDB);
            var mapper = ConfigurarAutoMapper();

            contexto.Generos.Add(new Genero() { Name = "Género 1" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreDB);
            var controller = new GeneroController(contexto2, mapper);

            var id = 1;

            var respuesta = await controller.Delete(id);
            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado.StatusCode);

            var contexto3 = ConstruirContext(nombreDB);
            var existe = await contexto3.Generos.AnyAsync();
            Assert.IsFalse(existe);
        }
    }
}
