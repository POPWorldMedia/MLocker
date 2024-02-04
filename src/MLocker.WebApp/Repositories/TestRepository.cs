using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MLocker.Core.Models;

namespace MLocker.WebApp.Repositories
{
	public interface ITestRepository
	{
		Task<Tuple<bool, HttpStatusCode, bool>> IsTestSuccess();
	}

	public class TestRepository : ITestRepository
	{
		private readonly HttpClient _httpClient;
		private readonly IConfig _config;

		public TestRepository(HttpClient httpClient, IConfig config)
		{
			_httpClient = httpClient;
			_config = config;
		}

		public async Task<Tuple<bool, HttpStatusCode, bool>> IsTestSuccess()
		{
			var apiKey = await _config.GetApiKey();
			if (string.IsNullOrEmpty(apiKey))
				return Tuple.Create(false, HttpStatusCode.Unauthorized, false);
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiKey);
			var response = await _httpClient.GetAsync(ApiPaths.Test);
			var isGuest = response.Headers.Contains("X-Is-Guest");
			return Tuple.Create(response.IsSuccessStatusCode, response.StatusCode, isGuest);
		}
	}
}