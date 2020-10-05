using System.Net.Http;

namespace MLocker.WebApp.Repositories
{
    public class SongRepository
    {
        private readonly HttpClient _httpClient;

        public SongRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
    }
}