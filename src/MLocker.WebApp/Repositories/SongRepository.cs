using System.Collections.Generic;
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
        Task<IEnumerable<Song>> GetAllSongs();
        string GetSongUrl(int fileID);
        Task IncrementPlayCount(int fileID);
    }

    public class SongRepository : ISongRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IConfig _config;

        public SongRepository(HttpClient httpClient, IConfig config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<IEnumerable<Song>> GetAllSongs()
        {
            var apiKey = await _config.GetApiKey();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiKey);
            var response = await _httpClient.GetStringAsync("/GetAllSongs");
            var allSongs = JsonSerializer.Deserialize<IEnumerable<Song>>(response, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            return allSongs;
        }

        public string GetSongUrl(int fileID)
        {
            var url = $"/GetSong/{fileID}";
            return url;
        }

        public async Task IncrementPlayCount(int fileID)
        {
	        var apiKey = await _config.GetApiKey();
	        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiKey);
	        var data = new {FileID = fileID};
	        await _httpClient.PostAsJsonAsync("/IncrementPlayCount", data);
        }
    }
}