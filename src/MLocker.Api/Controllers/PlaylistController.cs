using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MLocker.Api.Services;
using MLocker.Core.Models;

namespace MLocker.Api.Controllers
{
	public class PlaylistController : Controller
	{
		private readonly IPlaylistService _playlistService;

		public PlaylistController(IPlaylistService playlistService)
		{
			_playlistService = playlistService;
		}

		[ApiAuth]
		[HttpPost(ApiPaths.CreatePlaylist)]
		public async Task<IActionResult> CreatePlaylist([FromBody] PlaylistDefinition playlistDefinition)
		{
			var result = await _playlistService.CreatePlaylistDefinition(playlistDefinition);
			return Json(result);
		}

		[ApiAuth]
		[HttpGet(ApiPaths.GetAllPlaylistDefinitions)]
		public async Task<IActionResult> GetAllPlaylistDefinitions()
		{
			var version = await _playlistService.GetPlaylistVersion();
			var playlistDefinitions = await _playlistService.GetAllPlaylistDefinitions();
			var payload = new PlaylistPayload {Version = version, PlaylistDefinitions = playlistDefinitions};
			return Json(payload);
		}

		[ApiAuth]
		[HttpPost(ApiPaths.UpdatePlaylist)]
		public async Task<IActionResult> UpdatePlaylist([FromBody] PlaylistDefinition playlistDefinition)
		{
			await _playlistService.UpdatePlaylist(playlistDefinition);
			return Ok();
		}

		[ApiAuth]
		[HttpDelete(ApiPaths.DeletePlaylist + "/{id}")]
		public async Task<IActionResult> DeletePlaylist(int id)
		{
			await _playlistService.DeletePlaylist(id);
			return Ok();
		}

		[ApiAuth]
		[HttpGet(ApiPaths.GetPlaylistVersion)]
		public async Task<IActionResult> GetPlaylistVersion()
		{
			var version = await _playlistService.GetPlaylistVersion();
			return Content(version, "text/plain");
		}
	}
}