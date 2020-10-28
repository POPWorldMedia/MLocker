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
		Task<Playlist> CreateNewPlaylistDefinition(string title);
		Task<List<Playlist>> GetAllPlaylists();
		Task AddSongToEndOfPlaylist(Playlist playlist, Song song);
		Task UpdatePlaylist(Playlist playlist);
		Task DeletePlaylist(Playlist playlist);
	}

	public class PlaylistService : IPlaylistService
	{
		private readonly IPlaylistTransformer _playlistTransformer;
		private readonly IPlaylistRepository _playlistRepository;
		private readonly ISongRepository _songRepository;
		private readonly ISpinnerService _spinnerService;
		private static List<Playlist> _allPlaylists;

		public PlaylistService(IPlaylistTransformer playlistTransformer, IPlaylistRepository playlistRepository, ISongRepository songRepository, ISpinnerService spinnerService)
		{
			_playlistTransformer = playlistTransformer;
			_playlistRepository = playlistRepository;
			_songRepository = songRepository;
			_spinnerService = spinnerService;
		}

		public async Task<Playlist> CreateNewPlaylistDefinition(string title)
		{
			_allPlaylists = null;
			var playlistDefinition = await _playlistRepository.CreateNewPlaylistDefinition(title);
			var playlist = new Playlist
			{
				PlaylistID = playlistDefinition.PlaylistID,
				Title = playlistDefinition.Title,
				Songs = new List<Song>()
			};
			return playlist;
		}

		private async Task UpdatePlaylists()
		{
			await _playlistRepository.UpdatePlaylists();
			var definitions = await _playlistRepository.GetAllPlaylistDefinitions();
			var songs = await _songRepository.GetAllSongs();
			var playlists = _playlistTransformer.PlaylistDefinitionsToPlaylists(definitions, songs);
			_allPlaylists = playlists.OrderBy(x => x.Title).ToList();
		}

		public async Task<List<Playlist>> GetAllPlaylists()
		{
			try
			{
				_spinnerService.Show();
				if (_allPlaylists == null)
				{
					await UpdatePlaylists();
				}
			}
			catch
			{
				// TODO: error handling
			}
			finally
			{
				_spinnerService.Hide();
			}

			return _allPlaylists;
		}

		public async Task AddSongToEndOfPlaylist(Playlist playlist, Song song)
		{
			playlist.Songs.Add(song);
			var playlistDefinition = ConvertPlaylistToPlaylistDefinition(playlist);
			await _playlistRepository.UpdatePlaylist(playlistDefinition);
		}

		public async Task UpdatePlaylist(Playlist playlist)
		{
			var playlistDefinition = ConvertPlaylistToPlaylistDefinition(playlist);
			await _playlistRepository.UpdatePlaylist(playlistDefinition);
		}

		private PlaylistDefinition ConvertPlaylistToPlaylistDefinition(Playlist playlist)
		{
			return new PlaylistDefinition
			{
				PlaylistID = playlist.PlaylistID,
				Title = playlist.Title,
				SongIDs = playlist.Songs.Select(x => x.FileID).ToList()
			};
		}

		public async Task DeletePlaylist(Playlist playlist)
		{
			await _playlistRepository.DeletePlaylist(playlist.PlaylistID);
			_allPlaylists?.Remove(playlist);
		}
	}
}