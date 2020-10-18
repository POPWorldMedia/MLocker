using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MLocker.Api.Repositories;
using MLocker.Core.Models;

namespace MLocker.Api.Services
{
	public interface IPlaylistService
	{
		Task<PlaylistDefinition> CreatePlaylistDefinition(PlaylistDefinition playlistDefinition);
		Task<List<PlaylistDefinition>> GetAllPlaylistDefinitions();
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
			var newPlaylistDefinition = await _playlistRepository.CreatePlaylistDefinition(playlistDefinition.Title);
			if (playlistDefinition.PlaylistFiles != null && playlistDefinition.PlaylistFiles.Count > 0)
				foreach (var item in playlistDefinition.PlaylistFiles)
					await _playlistRepository.CreatePlaylistFile(item);
			return newPlaylistDefinition;
		}

		public async Task<List<PlaylistDefinition>> GetAllPlaylistDefinitions()
		{
			var definitions = await _playlistRepository.GetAllPlaylistDefinitions();
			var fileIDs = await _playlistRepository.GetAllPlaylistFiles();
			var fileIDList = fileIDs.ToList();
			var definitionList = definitions.ToList();
			foreach (var definition in definitionList)
			{
				definition.PlaylistFiles = fileIDList.Where(x => x.PlaylistID == definition.PlaylistID).ToList();
			}
			return definitionList.ToList();
		}
	}
}