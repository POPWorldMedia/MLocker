using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using MLocker.Core.Models;

namespace MLocker.WebApp.Repositories
{
	public interface IPlaylistRepository
	{
		Task UpdatePlaylists();
		Task<List<PlaylistDefinition>> GetAllPlaylistDefinitions();
		Task<PlaylistDefinition> CreateNewPlaylistDefinition(string title);
		Task UpdatePlaylist(PlaylistDefinition playlistDefinition);
		Task DeletePlaylist(int playlistID);
		Task<string> GetRemotePlaylistVersion();
	}

	public class PlaylistRepository : IPlaylistRepository
	{
		private readonly HttpClient _httpClient;
		private readonly IConfig _config;
		private readonly ILocalStorageService _localStorageService;
		private static List<PlaylistDefinition> _allPlaylistDefinitions;
		private const string PlaylistVersionKey = "PlaylistVersionKey";

		public PlaylistRepository(HttpClient httpClient, IConfig config, ILocalStorageService localStorageService)
		{
			_httpClient = httpClient;
			_config = config;
			_localStorageService = localStorageService;
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
			var playlistVersion = await _localStorageService.GetItemAsStringAsync(PlaylistVersionKey);
			var remotePlaylistVersion = await GetRemotePlaylistVersion();
			var isNewVersion = playlistVersion != remotePlaylistVersion;
			_httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = isNewVersion };
			var apiKey = await _config.GetApiKey();
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiKey);
			var response = await _httpClient.GetStringAsync(ApiPaths.GetAllPlaylistDefinitions);
			var payload = JsonSerializer.Deserialize<PlaylistPayload>(response, new JsonSerializerOptions(JsonSerializerDefaults.Web));
			_allPlaylistDefinitions = payload.PlaylistDefinitions.ToList();
			if (isNewVersion)
				await _localStorageService.SetItemAsync(PlaylistVersionKey, payload.Version);
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

		public async Task DeletePlaylist(int playlistID)
		{
			var apiKey = await _config.GetApiKey();
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiKey);
			string uri = $"{ApiPaths.DeletePlaylist}/{playlistID}";
			await _httpClient.DeleteAsync(uri);
		}

		public async Task<string> GetRemotePlaylistVersion()
		{
			var apiKey = await _config.GetApiKey();
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiKey);
			var playlistVersion = await _httpClient.GetStringAsync(ApiPaths.GetPlaylistVersion);
			return playlistVersion;
		}
	}
}