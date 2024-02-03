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
        private readonly ILocalStorageRepository _localStorageRepository;

        public UploadRepository(HttpClient httpClient, IConfig config, ILocalStorageRepository localStorageRepository)
        {
            _httpClient = httpClient;
            _config = config;
            _localStorageRepository = localStorageRepository;
        }

        public async Task<bool> UploadFile(string fileName, Stream stream)
        {
	        if (stream.Length == 0)
		        return false;
            using var content = new MultipartFormDataContent();
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            using var streamContent = new StreamContent(stream, Convert.ToInt32(stream.Length));
            fileName = fileName.Replace("\"", String.Empty);
            content.Add(streamContent, "file", fileName);

            var apiKey = await _config.GetApiKey();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiKey);
            using var response = await _httpClient.PostAsync(ApiPaths.Upload, content);
            streamContent.Dispose();
            content.Dispose();
            var result = response.IsSuccessStatusCode;
            response.Dispose();
            await _localStorageRepository.SetItem(SongRepository.SongListVersionKey, string.Empty);
            return result;
        }
    }
}