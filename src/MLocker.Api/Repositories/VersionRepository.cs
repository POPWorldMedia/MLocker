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
		Task SavePlaylistVersion(string songListVersion);
		Task<string> GetPlaylistVersion();
	}

	public class VersionRepository : IVersionRepository
	{
		private readonly IConfig _config;

		public VersionRepository(IConfig config)
		{
			_config = config;
		}

		private const string SongListVersionFileName = "SongListVersion.txt";
		private const string PlaylistVersionFileName = "PlaylistVersion.txt";
		private const string ContainerName = "music";

		public async Task SaveSongListVersion(string songListVersion)
		{
			await SaveVersion(SongListVersionFileName, songListVersion);
		}

		public async Task<string> GetSongListVersion()
		{
			return await GetVersion(SongListVersionFileName);
		}

		public async Task SavePlaylistVersion(string songListVersion)
		{
			await SaveVersion(PlaylistVersionFileName, songListVersion);
		}

		public async Task<string> GetPlaylistVersion()
		{
			return await GetVersion(PlaylistVersionFileName);
		}

		private async Task SaveVersion(string fileName, string songListVersion)
		{
			var stream = new MemoryStream(Encoding.UTF8.GetBytes(songListVersion));
			var serviceClient = new BlobServiceClient(_config.StorageConnectionString);
			var containerClient = serviceClient.GetBlobContainerClient(ContainerName);
			var blobClient = containerClient.GetBlobClient(fileName);
			await blobClient.DeleteIfExistsAsync();
			await blobClient.UploadAsync(stream, new BlobHttpHeaders {ContentType = "text/plain"});
		}

		private async Task<string> GetVersion(string fileName)
		{
			var serviceClient = new BlobServiceClient(_config.StorageConnectionString);
			var containerClient = serviceClient.GetBlobContainerClient(ContainerName);
			var blobClient = containerClient.GetBlobClient(fileName);
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