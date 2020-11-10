namespace MLocker.Core.Models
{
	public static class ApiPaths
	{
		public const string CreatePlaylist = "/CreatePlaylist";
		public const string GetAllPlaylistDefinitions = "/GetAllPlaylistDefinitions";
		public const string UpdatePlaylist = "/UpdatePlaylist";
		/// <summary>
		/// Append with "/{id}"
		/// </summary>
		public const string DeletePlaylist = "/DeletePlaylist";
		public const string Test = "/Test";
		public const string GetAllSongs = "/GetAllSongs";
		/// <summary>
		/// Append with "/{id}"
		/// </summary>
		public const string GetSong = "/GetSong";
		/// <summary>
		/// Append with "/{id}"
		/// </summary>
		public const string GetWholeSong = "/GetWholeSong";
		public const string GetImage = "/GetImage";
		public const string IncrementPlayCount = "/IncrementPlayCount";
		public const string Upload = "/Upload";
		public const string GetSongListVersion = "/GetSongListVersion";
		public const string GetPlaylistVersion = "/GetPlaylistVersion";
	}
}