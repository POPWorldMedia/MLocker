using System;
using System.Collections.Generic;
using System.Linq;
using MLocker.Core.Models;

namespace MLocker.Core.Services
{
	public interface IPlaylistTransformer
	{
		List<Playlist> PlaylistDefinitionsToPlaylists(List<PlaylistDefinition> playlistDefinitions, List<Song> songs);
		List<Song> Shuffle(List<Song> songList);
	}

	public class PlaylistTransformer : IPlaylistTransformer
	{
		public List<Playlist> PlaylistDefinitionsToPlaylists(List<PlaylistDefinition> playlistDefinitions, List<Song> songs)
		{
			var playlists = new List<Playlist>();
			foreach (var playlistDefinition in playlistDefinitions)
			{
				var songIDs = playlistDefinition.SongIDs;
				var theSongList = songIDs.Join(songs, 
					songID => songID, 
					song => song.FileID, 
					(songID, song) => song).ToList();
				var playlist = new Playlist
				{
					PlaylistID = playlistDefinition.PlaylistID,
					Title = playlistDefinition.Title,
					Songs = theSongList
				};
				playlists.Add(playlist);
			}
			return playlists.OrderBy(x => x.Title).ToList();
		}

		public List<Song> Shuffle(List<Song> songList)
		{
			var shuffled = songList.OrderBy(x => Guid.NewGuid()).ToList();
			return shuffled;
		}
	}
}