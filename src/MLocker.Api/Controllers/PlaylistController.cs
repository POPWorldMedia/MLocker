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
		public async Task<IActionResult> CreatePlaylist([FromBody]PlaylistDefinition playlistDefinition)
		{
			var result = await _playlistService.CreatePlaylistDefinition(playlistDefinition);
			return Json(result);
		}

		[ApiAuth]
		[HttpGet(ApiPaths.GetAllPlaylistDefinitions)]
		public async Task<IActionResult> GetAllPlaylistDefinitions()
		{
			var result = await _playlistService.GetAllPlaylistDefinitions();
			return Json(result);
		}
	}
}