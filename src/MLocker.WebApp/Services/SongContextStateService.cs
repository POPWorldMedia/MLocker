using MLocker.Core.Models;

namespace MLocker.WebApp.Services
{
	public interface ISongContextStateService
	{
		Song Song { get; set; }
		string ContextMenuID { get; set; }
		Playlist LastUsedPlaylist { get; set; }
		bool IsToAlbumLoad { get; set; }
		bool IsToArtistLoad { get; set; }
	}

	public class SongContextStateService : ISongContextStateService
	{
		private static Song _song;
		private static string _contextMenuID;
		private static Playlist _lastUsedPlaylist;
		private static bool _isToAlbumLoad;
		private static bool _isToArtistLoad;

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

		public bool IsToAlbumLoad
		{
			get => _isToAlbumLoad;
			set => _isToAlbumLoad = value;
		}

		public bool IsToArtistLoad
		{
			get => _isToArtistLoad;
			set => _isToArtistLoad = value;
		}
	}
}