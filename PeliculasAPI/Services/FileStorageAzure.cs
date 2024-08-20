using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace PeliculasAPI.Services
{
    public class FileStorageAzure : IFileStorage
    {
        private readonly string connectionString;
        public FileStorageAzure(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("AzureStorage");
        }
        public async Task BorrarArchivo(string ruta, string contenedor)
        {
            // // // // // MÉTODO PARA ELIMINAR IMAGEN DE AZURE // // // // //

            if (string.IsNullOrEmpty(ruta))
            {
                return;
            }

            var cliente = new BlobContainerClient(connectionString, contenedor);
            await cliente.CreateIfNotExistsAsync();
            var archivo = Path.GetFileName(ruta);
            var blob = cliente.GetBlobClient(archivo);
            await blob.DeleteIfExistsAsync();

            // // // // // MÉTODO PARA ELIMINAR IMAGEN DE AZURE // // // // //
        }

        public async Task<string> EditarArchivo(byte[] contenido, string extension,
            string contenedor, string ruta, string contentType)
        {
            await BorrarArchivo(ruta, contenedor);
            return await GuardarArchivo(contenido, extension, contenedor, contentType);
        }

        public async Task<string> GuardarArchivo(byte[] contenido, string extension, 
            string contenedor, string contentType)
        {
            // // // // // MÉTODO PARA EMPUJAR IMAGEN A AZURE // // // // //

            var cliente = new BlobContainerClient(connectionString, contenedor);
            await cliente.CreateIfNotExistsAsync();

            cliente.SetAccessPolicy(PublicAccessType.Blob);

            var archivoNombre = $"{ Guid.NewGuid()}{extension}";
            var blob = cliente.GetBlobClient(archivoNombre);

            var blobUploadOptions = new BlobUploadOptions();
            var blobHttpHeader = new BlobHttpHeaders();
            blobHttpHeader.ContentType = contentType;

            blobUploadOptions.HttpHeaders = blobHttpHeader;
            await blob.UploadAsync(new BinaryData(contenido), blobUploadOptions);

            return blob.Uri.ToString();

            // // // // // MÉTODO PARA EMPUJAR IMAGEN A AZURE // // // // //
        }
    }
}
