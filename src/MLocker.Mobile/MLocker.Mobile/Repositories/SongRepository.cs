using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using MLocker.Core.Models;

namespace MLocker.Mobile.Repositories
{
	public interface ISongRepository
	{
		Task<List<Song>> GetAllSongs();
	}

	public class SongRepository : ISongRepository
	{
		private readonly HttpClient _httpClient;

		public SongRepository()
		{
			var handler = new HttpClientHandler();
			handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
			{
				if (cert.Issuer.Equals("CN=localhost"))
					return true;
				return errors == System.Net.Security.SslPolicyErrors.None;
			};
			_httpClient = new HttpClient(handler) {BaseAddress = new Uri("https://10.0.2.2:5001") };
		}

		public async Task<List<Song>> GetAllSongs()
		{
			var apiKey = "123";
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiKey);
			var test = await _httpClient.GetAsync(ApiPaths.GetAllSongs);
			var response = await _httpClient.GetStringAsync(ApiPaths.GetAllSongs);
			var allSongs = JsonSerializer.Deserialize<IEnumerable<Song>>(response, new JsonSerializerOptions(JsonSerializerDefaults.Web));
			return allSongs.ToList();
		}
    }
}