﻿using System.Collections.Generic;
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
		Task AddSongToEndOfPlaylist(Playlist playlist, Song song);
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

		public async Task<PlaylistDefinition> CreateNewPlaylistDefinition(string title)
		{
			_allPlaylists = null;
			return await _playlistRepository.CreateNewPlaylistDefinition(title);
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
			var orderIndex = playlist.Songs.Count * 2;
			var playlistFile = new PlaylistFile {FileID = song.FileID, PlaylistID = playlist.PlaylistID, SortOrder = orderIndex};
			await _playlistRepository.CreatePlaylistFile(playlistFile);
			playlist.Songs.Add(song);
		}
	}
}