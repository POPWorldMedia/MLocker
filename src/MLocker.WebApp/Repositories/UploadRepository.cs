using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MLocker.WebApp.Repositories
{
    public interface IUploadRepository
    {
        Task UploadFile(string fileName, Stream stream);
    }

    public class UploadRepository : IUploadRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IConfig _config;

        public UploadRepository(HttpClient httpClient, IConfig config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task UploadFile(string fileName, Stream stream)
        {
            var content = new MultipartFormDataContent();
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            content.Add(new StreamContent(stream, Convert.ToInt32(stream.Length)), "file", fileName);

            var baseUrl = await _config.GetBaseApiUrl();
            await _httpClient.PostAsync($"{baseUrl}/upload", content);
        }
    }
}