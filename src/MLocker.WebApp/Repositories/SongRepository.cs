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
    public interface ISongRepository
    {
        Task UpdateSongs();
        Task<List<Song>> GetAllSongs();
        string GetSongUrl(int fileID);
        Task IncrementPlayCount(int fileID);
    }

    public class SongRepository : ISongRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IConfig _config;
        private static List<Song> _allSongs;

        public SongRepository(HttpClient httpClient, IConfig config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task UpdateSongs()
        {
	        _allSongs = null;
	        await GetAllSongs();
        }

        public async Task<List<Song>> GetAllSongs()
        {
	        if (_allSongs != null)
		        return _allSongs;
            var apiKey = await _config.GetApiKey();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiKey);
            var response = await _httpClient.GetStringAsync(ApiPaths.GetAllSongs);
            var allSongs = JsonSerializer.Deserialize<IEnumerable<Song>>(response, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            _allSongs = allSongs.ToList();
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
    }
}