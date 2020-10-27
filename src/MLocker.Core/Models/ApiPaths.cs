namespace MLocker.Core.Models
{
	public static class ApiPaths
	{
		public const string CreatePlaylist = "/CreatePlaylist";
		public const string GetAllPlaylistDefinitions = "/GetAllPlaylistDefinitions";
		public const string UpdatePlaylist = "/UpdatePlaylist";
		public const string Test = "/Test";
		public const string GetAllSongs = "/GetAllSongs";
		/// <summary>
		/// Append with "/{id}"
		/// </summary>
		public const string GetSong = "/GetSong";
		public const string GetImage = "/GetImage";
		public const string IncrementPlayCount = "/IncrementPlayCount";
		public const string Upload = "/Upload";
	}
}