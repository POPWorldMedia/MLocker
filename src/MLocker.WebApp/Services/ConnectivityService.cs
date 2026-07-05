using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace MLocker.WebApp.Services
{
	public interface IConnectivityService
	{
		bool IsOnline { get; }
		event Action OnChange;
	}

	public class ConnectivityService : IConnectivityService
	{
		private readonly IJSRuntime _jsRuntime;
		private static Action<bool> _setOnlineAction;

		public bool IsOnline { get; private set; } = true;
		public event Action OnChange;

		public ConnectivityService(IJSRuntime jsRuntime)
		{
			_jsRuntime = jsRuntime;
			_setOnlineAction = SetOnline;
			_ = Initialize();
		}

		private async Task Initialize()
		{
			IsOnline = await _jsRuntime.InvokeAsync<bool>("IsOnline");
			await _jsRuntime.InvokeVoidAsync("RegisterConnectivityListener");
			OnChange?.Invoke();
		}

		private void SetOnline(bool isOnline)
		{
			if (IsOnline == isOnline)
				return;
			IsOnline = isOnline;
			OnChange?.Invoke();
		}

		[JSInvokable]
		public static void NotifyConnectivityChanged(bool isOnline)
		{
			_setOnlineAction?.Invoke(isOnline);
		}
	}
}
