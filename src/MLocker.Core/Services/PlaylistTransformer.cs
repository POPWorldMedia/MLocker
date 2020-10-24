using System;
using System.Collections.Generic;
using System.Linq;
using MLocker.Core.Models;

namespace MLocker.Core.Services
{
	public interface IPlaylistTransformer
	{
		List<Playlist> PlaylistDefinitionsToPlaylists(List<PlaylistDefinition> playlistDefinitions, List<Song> songs);
		List<Song> Shuffle(Dictionary<int, Song> songList);
	}

	public class PlaylistTransformer : IPlaylistTransformer
	{
		public List<Playlist> PlaylistDefinitionsToPlaylists(List<PlaylistDefinition> playlistDefinitions, List<Song> songs)
		{
			var playlists = new List<Playlist>();
			foreach (var playlistDefinition in playlistDefinitions)
			{
				var playlistFiles = playlistDefinition.PlaylistFiles.OrderBy(x => x.SortOrder);
				var playlistSongs = new List<Song>();
				foreach (var playlistFile in playlistFiles)
					playlistSongs.Add(songs.Single(x => x.FileID == playlistFile.FileID));
				var playlist = new Playlist
				{
					PlaylistID = playlistDefinition.PlaylistID,
					Title = playlistDefinition.Title,
					Songs = playlistSongs
				};
				playlists.Add(playlist);
			}
			return playlists.OrderBy(x => x.Title).ToList();
		}

		public List<Song> Shuffle(Dictionary<int, Song> songList)
		{
			var shuffled = songList.Select(x => x.Value).OrderBy(x => Guid.NewGuid()).ToList();
			return shuffled;
		}
	}
}