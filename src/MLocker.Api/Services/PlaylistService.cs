using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MLocker.Api.Models;
using MLocker.Api.Repositories;
using MLocker.Core.Models;

namespace MLocker.Api.Services
{
	public interface IPlaylistService
	{
		Task<PlaylistDefinition> CreatePlaylistDefinition(PlaylistDefinition playlistDefinition);
		Task<List<PlaylistDefinition>> GetAllPlaylistDefinitions();
		Task UpdatePlaylist(PlaylistDefinition playlist);
	}

	public class PlaylistService : IPlaylistService
	{
		private readonly IPlaylistRepository _playlistRepository;

		public PlaylistService(IPlaylistRepository playlistRepository)
		{
			_playlistRepository = playlistRepository;
		}

		public async Task<PlaylistDefinition> CreatePlaylistDefinition(PlaylistDefinition playlistDefinition)
		{
			var newEntity = await _playlistRepository.CreatePlaylistEntity(playlistDefinition.Title);
			var newPlaylistDefinition = new PlaylistDefinition
			{
				PlaylistID = newEntity.PlaylistID,
				Title = newEntity.Title,
				SongIDs = new List<int>()
			};
			return newPlaylistDefinition;
		}

		public async Task<List<PlaylistDefinition>> GetAllPlaylistDefinitions()
		{
			var entities = await _playlistRepository.GetAllPlaylistEntities();
			var list = entities.Select(x => new PlaylistDefinition
			{
				PlaylistID = x.PlaylistID,
				Title = x.Title,
				SongIDs = string.IsNullOrEmpty(x.SongsJson) ? new List<int>() : JsonSerializer.Deserialize<List<int>>(x.SongsJson)
			});
			return list.ToList();
		}

		public async Task UpdatePlaylist(PlaylistDefinition playlistDefinition)
		{
			var serializedSongIDs = JsonSerializer.Serialize(playlistDefinition.SongIDs);
			var entity = new PlaylistEntity
			{
				PlaylistID = playlistDefinition.PlaylistID,
				Title = playlistDefinition.Title,
				SongsJson = serializedSongIDs
			};
			await _playlistRepository.UpdatePlaylist(entity);
		}
	}
}