using System.Threading.Tasks;
using MLocker.WebApp.Repositories;

namespace MLocker.WebApp
{
    public interface IConfig
    {
        Task<string> GetApiKey();
        Task SetApiKey(string baseApiUrl);
    }

    public class Config : IConfig
    {
	    private readonly ILocalStorageRepository _localStorageRepository;
	    private const string ApiKeyKey = "ApiKeyKey";

        public Config(ILocalStorageRepository localStorageRepository)
        {
	        _localStorageRepository = localStorageRepository;
        }

        public async Task<string> GetApiKey()
        {
            var key = await _localStorageRepository.GetItem(ApiKeyKey);
            return key ?? string.Empty;
        }

        public async Task SetApiKey(string baseApiUrl)
        {
            await _localStorageRepository.SetItem(ApiKeyKey, baseApiUrl);
        }
    }
}