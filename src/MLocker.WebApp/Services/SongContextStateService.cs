using MLocker.Core.Models;

namespace MLocker.WebApp.Services
{
	public interface ISongContextStateService
	{
		Song Song { get; set; }
		string ContextMenuID { get; set; }
		Playlist LastUsedPlaylist { get; set; }
	}

	public class SongContextStateService : ISongContextStateService
	{
		private static Song _song;
		private static string _contextMenuID;
		private static Playlist _lastUsedPlaylist;

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

		public Playlist LastUsedPlaylist
		{
			get => _lastUsedPlaylist;
			set => _lastUsedPlaylist = value;
		}
	}
}