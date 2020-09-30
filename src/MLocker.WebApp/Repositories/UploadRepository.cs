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

        public UploadRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task UploadFile(string fileName, Stream stream)
        {
            var content = new MultipartFormDataContent();
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            content.Add(new StreamContent(stream, Convert.ToInt32(stream.Length)), "file", fileName);

            await _httpClient.PostAsync("https://localhost:44314/upload", content);
        }
    }
}