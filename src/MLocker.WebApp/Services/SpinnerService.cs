using System;

namespace MLocker.WebApp.Services
{
	public interface ISpinnerService
	{
		void Show();
		void Hide();
		event Action OnShow;
		event Action OnHide;
	}

	public class SpinnerService : ISpinnerService
	{
		public void Show() => OnShow?.Invoke();

		public void Hide() => OnHide?.Invoke();

		public event Action OnShow;
		public event Action OnHide;
	}
}