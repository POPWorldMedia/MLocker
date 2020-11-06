using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using MLocker.Core.Models;

namespace MLocker.WebApp.Repositories
{
    public interface ISongRepository
    {
        Task UpdateSongs();
        Task<List<Song>> GetAllSongs();
        string GetSongUrl(int fileID);
        Task IncrementPlayCount(int fileID);
        Task<string> GetRemoteSongListVersion();
    }

    public class SongRepository : ISongRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IConfig _config;
        private readonly ILocalStorageService _localStorageService;
        private static List<Song> _allSongs;
        private static SemaphoreSlim _updateLocker = new SemaphoreSlim(1, 1);
        private const string SongListVersionKey = "SongListVersionKey";

        public SongRepository(HttpClient httpClient, IConfig config, ILocalStorageService localStorageService)
        {
            _httpClient = httpClient;
            _config = config;
            _localStorageService = localStorageService;
        }

        public async Task UpdateSongs()
        {
	        _allSongs = null;
	        await GetAllSongs();
        }

        public async Task<List<Song>> GetAllSongs()
        {
	        await _updateLocker.WaitAsync(); // not crazy about this, but first load this can happen concurrently because of component init timing
	        if (_allSongs != null && _allSongs.Any())
	        {
		        _updateLocker.Release();
		        return _allSongs;
	        }

	        try
	        {
		        var songListVersion = await _localStorageService.GetItemAsStringAsync(SongListVersionKey);
		        var remoteSongListVersion = await GetRemoteSongListVersion();
		        var isNewVersion = songListVersion != remoteSongListVersion;
		        _httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue {NoCache = isNewVersion};
		        var apiKey = await _config.GetApiKey();
		        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiKey);
		        var response = await _httpClient.GetStringAsync(ApiPaths.GetAllSongs);
		        var payload = JsonSerializer.Deserialize<SongListPayload>(response, new JsonSerializerOptions(JsonSerializerDefaults.Web));
		        _allSongs = payload.Songs.ToList();
		        if (isNewVersion)
			        await _localStorageService.SetItemAsync(SongListVersionKey, payload.Version);
	        }
	        catch
	        {
		        throw; // TODO: error handling
	        }
	        finally
	        {
		        _updateLocker.Release();
	        }

	        return _allSongs;
        }

        public string GetSongUrl(int fileID)
        {
            var url = $"{ApiPaths.GetSong}/{fileID}";
            return url;
        }

        public async Task IncrementPlayCount(int fileID)
        {
	        var song = _allSongs.SingleOrDefault(x => x.FileID == fileID);
	        if (song != null)
		        song.PlayCount++;
	        var apiKey = await _config.GetApiKey();
	        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiKey);
	        var data = new {FileID = fileID};
	        await _httpClient.PostAsJsonAsync(ApiPaths.IncrementPlayCount, data);
        }

        public async Task<string> GetRemoteSongListVersion()
        {
            var songListVersion = await _httpClient.GetStringAsync(ApiPaths.GetSongListVersion);
	        return songListVersion;
        }
    }
}