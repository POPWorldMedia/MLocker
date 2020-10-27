using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using MLocker.Core.Models;

namespace MLocker.WebApp.Repositories
{
	public interface IPlaylistRepository
	{
		Task UpdatePlaylists();
		Task<List<PlaylistDefinition>> GetAllPlaylistDefinitions();
		Task<PlaylistDefinition> CreateNewPlaylistDefinition(string title);
		Task UpdatePlaylist(PlaylistDefinition playlistDefinition);
	}

	public class PlaylistRepository : IPlaylistRepository
	{
		private readonly HttpClient _httpClient;
		private readonly IConfig _config;
		private static List<PlaylistDefinition> _allPlaylistDefinitions;

		public PlaylistRepository(HttpClient httpClient, IConfig config)
		{
			_httpClient = httpClient;
			_config = config;
		}

		public async Task UpdatePlaylists()
		{
			_allPlaylistDefinitions = null;
			await GetAllPlaylistDefinitions();
		}

		public async Task<List<PlaylistDefinition>> GetAllPlaylistDefinitions()
		{
			if (_allPlaylistDefinitions != null)
				return _allPlaylistDefinitions;
			var apiKey = await _config.GetApiKey();
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiKey);
			var response = await _httpClient.GetStringAsync(ApiPaths.GetAllPlaylistDefinitions);
			var allPlaylistDefinitions = JsonSerializer.Deserialize<IEnumerable<PlaylistDefinition>>(response, new JsonSerializerOptions(JsonSerializerDefaults.Web));
			_allPlaylistDefinitions = allPlaylistDefinitions.ToList();
			return _allPlaylistDefinitions;
		}

		public async Task<PlaylistDefinition> CreateNewPlaylistDefinition(string title)
		{
			var apiKey = await _config.GetApiKey();
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiKey);
			var payload = new PlaylistDefinition {Title = title};
			var response = await _httpClient.PostAsJsonAsync(ApiPaths.CreatePlaylist, payload);
			var responsePayload = await response.Content.ReadAsStringAsync();
			var playlistDefinition = JsonSerializer.Deserialize<PlaylistDefinition>(responsePayload);
			_allPlaylistDefinitions?.Add(playlistDefinition);
			return playlistDefinition;
		}

		public async Task UpdatePlaylist(PlaylistDefinition playlistDefinition)
		{
			var apiKey = await _config.GetApiKey();
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiKey);
			await _httpClient.PostAsJsonAsync(ApiPaths.UpdatePlaylist, playlistDefinition);
		}
	}
}