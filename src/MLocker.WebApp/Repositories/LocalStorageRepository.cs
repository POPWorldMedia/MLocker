using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace MLocker.WebApp.Repositories
{
	public interface ILocalStorageRepository
	{
		Task SetItem(string key, string value);
		Task<string> GetItem(string key);
	}

	public class LocalStorageRepository : ILocalStorageRepository
	{
		private readonly IJSRuntime _jsRuntime;

		public LocalStorageRepository(IJSRuntime jsRuntime)
		{
			_jsRuntime = jsRuntime;
		}

		public async Task SetItem(string key, string value)
		{
			await _jsRuntime.InvokeVoidAsync("SetStorageItem", key, value);
		}

		public async Task<string> GetItem(string key)
		{
			var result = await _jsRuntime.InvokeAsync<string>("GetStorageItem", key);
			return result;
		}
	}
}