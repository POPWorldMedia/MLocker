using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MLocker.Core.Models;

namespace MLocker.WebApp.Repositories
{
    public interface IUploadRepository
    {
	    Task<bool> UploadFile(string fileName, Stream stream);
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

        public async Task<bool> UploadFile(string fileName, Stream stream)
        {
	        if (stream.Length == 0)
		        return false;
            var content = new MultipartFormDataContent();
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            content.Add(new StreamContent(stream, Convert.ToInt32(stream.Length)), "file", fileName);

            var apiKey = await _config.GetApiKey();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiKey);
            var response = await _httpClient.PostAsync(ApiPaths.Upload, content);
            return response.IsSuccessStatusCode;
        }
    }
}