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
            return await _localStorageService.GetItemAsStringAsync(ApiKeyKey);
        }

        public async Task SetApiKey(string baseApiUrl)
        {
            await _localStorageService.SetItemAsync(ApiKeyKey, baseApiUrl);
        }
    }
}