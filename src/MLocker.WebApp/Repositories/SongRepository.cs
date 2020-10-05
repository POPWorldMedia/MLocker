using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MLocker.Core.Models;

namespace MLocker.WebApp.Repositories
{
    public interface ISongRepository
    {
        Task<IEnumerable<Song>> GetAllSongs();
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
            var baseUrl = await _config.GetBaseApiUrl();
            var response = await _httpClient.GetStringAsync($"{baseUrl}/GetAllSongs");
            var allSongs = JsonSerializer.Deserialize<IEnumerable<Song>>(response, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            return allSongs;
        }
    }
}