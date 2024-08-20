using PeliculasAPI.Validations;
using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
    public class ActorCreacionDTO: ActorPatchDTO
    {        
        [FileWeightValidation(pesoMaximoEnMegaBytes:4)]
        [FileTypeValidation(groupFileType: GroupFileType.Image)]
        public IFormFile Photo { get; set; }
    }
}
