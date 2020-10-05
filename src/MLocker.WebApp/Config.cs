using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace MLocker.WebApp
{
    public interface IConfig
    {
        Task<string> GetBaseApiUrl();
        Task SetBaseApiUrl(string baseApiUrl);
    }

    public class Config : IConfig
    {
        private readonly ILocalStorageService _localStorageService;
        private const string BaseApiUrlKey = "BaseApiUrl";

        public Config(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public async Task<string> GetBaseApiUrl()
        {
            return await _localStorageService.GetItemAsStringAsync(BaseApiUrlKey);
        }

        public async Task SetBaseApiUrl(string baseApiUrl)
        {
            await _localStorageService.SetItemAsync(BaseApiUrlKey, baseApiUrl);
        }
    }
}