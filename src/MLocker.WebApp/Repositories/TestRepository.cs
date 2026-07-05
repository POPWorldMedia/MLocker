using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MLocker.Core.Models;
using MLocker.WebApp.Services;

namespace MLocker.WebApp.Repositories
{
	public interface ITestRepository
	{
		// isSuccess, statusCode, isGuest, wasOffline
		Task<Tuple<bool, HttpStatusCode, bool, bool>> IsTestSuccess();
	}

	public class TestRepository : ITestRepository
	{
		private readonly HttpClient _httpClient;
		private readonly IConfig _config;
		private readonly IConnectivityService _connectivityService;

		public TestRepository(HttpClient httpClient, IConfig config, IConnectivityService connectivityService)
		{
			_httpClient = httpClient;
			_config = config;
			_connectivityService = connectivityService;
		}

		public async Task<Tuple<bool, HttpStatusCode, bool, bool>> IsTestSuccess()
		{
			var apiKey = await _config.GetApiKey();
			if (string.IsNullOrEmpty(apiKey))
				return Tuple.Create(false, HttpStatusCode.Unauthorized, false, false);

			// Being disconnected is not evidence the key is bad - don't force the API key prompt for it.
			if (!_connectivityService.IsOnline)
				return Tuple.Create(true, HttpStatusCode.OK, false, true);

			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(apiKey);
			try
			{
				var response = await _httpClient.GetAsync(ApiPaths.Test);
				var isGuest = response.Headers.Contains("X-Is-Guest");
				return Tuple.Create(response.IsSuccessStatusCode, response.StatusCode, isGuest, false);
			}
			catch (HttpRequestException)
			{
				// Reachable per the OS, but the API itself couldn't be reached - treat like offline, not a bad key.
				return Tuple.Create(true, HttpStatusCode.ServiceUnavailable, false, true);
			}
		}
	}
}
