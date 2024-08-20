using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Validations
{
    public class FileTypeValidation: ValidationAttribute
    {
        private readonly string[] tiposValidos;

        public FileTypeValidation(string[] tiposValidos)
        {
            this.tiposValidos = tiposValidos;
        }

        public FileTypeValidation(GroupFileType groupFileType)
        {
            if(groupFileType == GroupFileType.Image)
            {
                tiposValidos = new string[] { "image/jpeg", "image/png", "image/gif" };
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            IFormFile formFile = value as IFormFile;

            if (formFile == null)
            {
                return ValidationResult.Success;
            }

            if (!tiposValidos.Contains(formFile.ContentType))
            {
                return new ValidationResult($"El tipo del archivo debe ser uno de los siguientes: {string.Join(", ", tiposValidos)}");
            }

            return ValidationResult.Success;
        }
    }
}
