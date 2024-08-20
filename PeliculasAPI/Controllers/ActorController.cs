using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entities;
using PeliculasAPI.Helpers;
using PeliculasAPI.Services;

namespace PeliculasAPI.Controllers
{
    [ApiController]
    [Route("/api/actores")]
    public class ActorController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IFileStorage fileStorage;
        private readonly string contenedor = "actores";

        public ActorController(ApplicationDbContext context,
            IMapper mapper, 
            IFileStorage fileStorage)
            :base (context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorage = fileStorage;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            return await Get<Actor, ActorDTO>(paginacionDTO);
        }

        [HttpGet("{id}", Name = "obtenerActor")]
        public async Task<ActionResult<ActorDTO>> Get(int id)
        {
            return await Get<Actor, ActorDTO>(id);


            //var entidad = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            //if (entidad == null)
            //{
            //    return NotFound();
            //}

            //return mapper.Map<ActorDTO>(entidad);

            //var dto = mapper.Map<ActorDTO>(entidad);
            //return dto;                               ---> otra forma de retornar

        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            // En este caso, no utilizamos el CustomBaseController porque tenemos una lógica de programación muy particular

            var entidad = mapper.Map<Actor>(actorCreacionDTO);

            if (actorCreacionDTO.Photo != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await actorCreacionDTO.Photo.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(actorCreacionDTO.Photo.FileName);
                    entidad.Photo = await fileStorage.GuardarArchivo(contenido, extension, contenedor,
                        actorCreacionDTO.Photo.ContentType);
                }
            }

            context.Add(entidad);
            await context.SaveChangesAsync();

            var dto = mapper.Map<ActorDTO>(entidad);

            return new CreatedAtRouteResult("obtenerActor", new { id = entidad.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            var actorDB = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);
            if (actorDB == null) { return NotFound(); }

            actorDB = mapper.Map(actorCreacionDTO, actorDB);

            if (actorCreacionDTO.Photo != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await actorCreacionDTO.Photo.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(actorCreacionDTO.Photo.FileName);
                    actorDB.Photo = await fileStorage.EditarArchivo(contenido, extension, contenedor,
                        actorDB.Photo,
                        actorCreacionDTO.Photo.ContentType);
                }
            }

            //var entidad = mapper.Map<Actor>(actorCreacionDTO);
            //entidad.Id = id;

            //context.Entry(entidad).State = EntityState.Modified;

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<ActorPatchDTO> patchDocument)
        {
            return await Patch<Actor, ActorPatchDTO>(id, patchDocument);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Actor>(id);
        }
    }
}
