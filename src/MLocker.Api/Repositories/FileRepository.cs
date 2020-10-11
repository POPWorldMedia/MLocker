using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace MLocker.Api.Repositories
{
    public interface IFileRepository
    {
        Task SaveFile(string fileName, byte[] bytes);
        Task<Stream> GetFile(string fileName);
    }

    public class FileRepository : IFileRepository
    {
        private readonly IConfig _config;
        private const string ContainerName = "music";

        public FileRepository(IConfig config)
        {
            _config = config;
        }

        public async Task SaveFile(string fileName, byte[] bytes)
        {
            var memoryStream = new MemoryStream(bytes);
            var serviceClient = new BlobServiceClient(_config.StorageConnectionString);
            var containerClient = serviceClient.GetBlobContainerClient(ContainerName);
            await containerClient.DeleteBlobIfExistsAsync(fileName);
            await containerClient.UploadBlobAsync(fileName, memoryStream);
        }

        public async Task<Stream> GetFile(string fileName)
        {
            var serviceClient = new BlobServiceClient(_config.StorageConnectionString);
            var containerClient = serviceClient.GetBlobContainerClient(ContainerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            var stream = await blobClient.OpenReadAsync();
            return stream;
        }
    }
}