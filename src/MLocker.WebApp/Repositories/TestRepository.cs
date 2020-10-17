using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MLocker.WebApp.Repositories
{
	public interface ITestRepository
	{
		Task<Tuple<bool, HttpStatusCode>> IsTestSuccess();
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

		public async Task<Tuple<bool, HttpStatusCode>> IsTestSuccess()
		{
			var apiKey = await _config.GetApiKey();
			if (string.IsNullOrEmpty(apiKey))
				return Tuple.Create(false, HttpStatusCode.Unauthorized);
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiKey);
			var response = await _httpClient.GetAsync("/Test");
			return Tuple.Create(response.IsSuccessStatusCode, response.StatusCode);
		}
	}
}