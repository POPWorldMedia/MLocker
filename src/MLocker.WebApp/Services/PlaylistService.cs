using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MLocker.Core.Models;
using MLocker.Core.Services;
using MLocker.WebApp.Repositories;

namespace MLocker.WebApp.Services
{
	public interface IPlaylistService
	{
		Task<PlaylistDefinition> CreateNewPlaylistDefinition(string title);
		Task<List<Playlist>> GetAllPlaylists();
		Task UpdatePlaylists();
	}

	public class PlaylistService : IPlaylistService
	{
		private readonly IPlaylistTransformer _playlistTransformer;
		private readonly IPlaylistRepository _playlistRepository;
		private readonly ISongRepository _songRepository;
		private static List<Playlist> _allPlaylists;

		public PlaylistService(IPlaylistTransformer playlistTransformer, IPlaylistRepository playlistRepository, ISongRepository songRepository)
		{
			_playlistTransformer = playlistTransformer;
			_playlistRepository = playlistRepository;
			_songRepository = songRepository;
		}

		public async Task<PlaylistDefinition> CreateNewPlaylistDefinition(string title)
		{
			_allPlaylists = null;
			return await _playlistRepository.CreateNewPlaylistDefinition(title);
		}

		public async Task UpdatePlaylists()
		{
			await _playlistRepository.UpdatePlaylists();
			var definitions = await _playlistRepository.GetAllPlaylistDefinitions();
			var songs = await _songRepository.GetAllSongs();
			var playlists = _playlistTransformer.PlaylistDefinitionsToPlaylists(definitions, songs);
			_allPlaylists = playlists.OrderBy(x => x.Title).ToList();
		}

		public async Task<List<Playlist>> GetAllPlaylists()
		{
			if (_allPlaylists == null)
				await UpdatePlaylists();
			return _allPlaylists;
		}
	}
}