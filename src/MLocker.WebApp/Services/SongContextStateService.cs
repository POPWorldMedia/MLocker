using MLocker.Core.Models;

namespace MLocker.WebApp.Services
{
	public interface ISongContextStateService
	{
		Song Song { get; set; }
		string ContextMenuID { get; set; }
	}

	public class SongContextStateService : ISongContextStateService
	{
		private static Song _song;
		private static string _contextMenuID;

		public Song Song
		{
			get => _song;
			set => _song = value;
		}

		public string ContextMenuID
		{
			get => _contextMenuID;
			set => _contextMenuID = value;
		}
	}
}