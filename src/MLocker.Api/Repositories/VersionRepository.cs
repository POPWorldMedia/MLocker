using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace MLocker.Api.Repositories
{
	public interface IVersionRepository
	{
		Task SaveSongListVersion(string songListVersion);
		Task<string> GetSongListVersion();
	}

	public class VersionRepository : IVersionRepository
	{
		private readonly IConfig _config;

		public VersionRepository(IConfig config)
		{
			_config = config;
		}

		private const string SongListVersionFileName = "SongListVersion.txt";
		private const string ContainerName = "music";

		public async Task SaveSongListVersion(string songListVersion)
		{
			var stream = new MemoryStream(Encoding.UTF8.GetBytes(songListVersion));
			var serviceClient = new BlobServiceClient(_config.StorageConnectionString);
			var containerClient = serviceClient.GetBlobContainerClient(ContainerName);
			var blobClient = containerClient.GetBlobClient(SongListVersionFileName);
			await blobClient.DeleteIfExistsAsync();
			await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = "text/plain" });
		}

		public async Task<string> GetSongListVersion()
		{
			var serviceClient = new BlobServiceClient(_config.StorageConnectionString);
			var containerClient = serviceClient.GetBlobContainerClient(ContainerName);
			var blobClient = containerClient.GetBlobClient(SongListVersionFileName);
			var exists = blobClient.Exists();
			if (!exists.Value)
				return null;
			var response = await blobClient.DownloadAsync();
			using var streamReader = new StreamReader(response.Value.Content);
			var text = await streamReader.ReadToEndAsync();
			return text;
		}
	}
}