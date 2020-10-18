using MLocker.Core.Models;

namespace MLocker.WebApp.Services
{
	public interface ISongContextStateService
	{
		Song Song { get; set; }
	}

	public class SongContextStateService : ISongContextStateService
	{
		private static Song _song;

		public Song Song
		{
			get => _song;
			set => _song = value;
		}
	}
}