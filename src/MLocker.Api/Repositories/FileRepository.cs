using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;

namespace MLocker.Api.Repositories
{
    public interface IFileRepository
    {
        Task SaveFile(string fileName, byte[] bytes);
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
    }
}