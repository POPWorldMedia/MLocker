using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace MLocker.WebApp
{
    public interface IConfig
    {
        Task<string> GetApiKey();
        Task SetApiKey(string baseApiUrl);
    }

    public class Config : IConfig
    {
        private readonly ILocalStorageService _localStorageService;
        private const string ApiKeyKey = "ApiKeyKey";

        public Config(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public async Task<string> GetApiKey()
        {
            var key = await _localStorageService.GetItemAsStringAsync(ApiKeyKey);
            return key ?? string.Empty;
        }

        public async Task SetApiKey(string baseApiUrl)
        {
            await _localStorageService.SetItemAsync(ApiKeyKey, baseApiUrl);
        }
    }
}