using System.Collections.Generic;
using System.Linq;
using MLocker.Core.Models;

namespace MLocker.Core.Services
{
	public interface IPlaylistTransformer
	{
		List<Playlist> PlaylistDefinitionsToPlaylists(List<PlaylistDefinition> playlistDefinitions, List<Song> songs);
	}

	public class PlaylistTransformer : IPlaylistTransformer
	{
		public List<Playlist> PlaylistDefinitionsToPlaylists(List<PlaylistDefinition> playlistDefinitions, List<Song> songs)
		{
			var result = playlistDefinitions.Select(x =>
				new Playlist
				{
					Title = x.Title,
					PlaylistID = x.PlaylistID,
					Songs = songs.Where(s => x.PlaylistFiles.Select(p => p.FileID).Contains(s.FileID)).ToList()
				});
			return result.ToList();
		}
	}
}