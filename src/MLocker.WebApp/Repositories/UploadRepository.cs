using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using MLocker.Core.Models;

namespace MLocker.WebApp.Repositories
{
    public interface IUploadRepository
    {
        Task UploadFile(UploadContainer container);
    }

    public class UploadRepository : IUploadRepository
    {
        private readonly HttpClient _httpClient;

        public UploadRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task UploadFile(UploadContainer container)
        {
            var payload = JsonSerializer.Serialize(container);
            await _httpClient.PostAsync("http://localhost:7071/api/Upload", new StringContent(payload));
        }
    }
}