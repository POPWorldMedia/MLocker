using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MLocker.Api.Services;
using MLocker.Core.Models;

namespace MLocker.Api.Controllers
{
    [ApiController]
    public class SongController : ControllerBase
    {
        private readonly ISongService _songService;

        public SongController(ISongService songService)
        {
            _songService = songService;
        }

        [ApiAuth]
        [HttpGet(ApiPaths.GetAllSongs)]
        public async Task<IActionResult> GetAll()
        {
            var allSongs = await _songService.GetAll();
            var version = await _songService.GetSongListVersion();
            var payload = new SongListPayload {Songs = allSongs, Version = version};
            return Ok(payload);
        }

        [HttpGet(ApiPaths.GetSong + "/{id}")]
        public async Task<IActionResult> GetSong(int id)
        {
            var (streamResult, song) = await _songService.GetSong(id);
            if (streamResult == null || song == null)
                return StatusCode(404);
            var mediaType = song.FileName.EndsWith("mp3") ? "audio/mpeg" : "audio/mp4";
            Response.RegisterForDispose(streamResult);
            return File(streamResult.Stream, mediaType, true);
        }

        [HttpGet(ApiPaths.GetWholeSong + "/{id}")]
        public async Task<IActionResult> GetWholeSong(int id)
        {
	        var (streamResult, song) = await _songService.GetSong(id);
	        if (streamResult == null || song == null)
		        return StatusCode(404);
	        var mediaType = song.FileName.EndsWith("mp3") ? "audio/mpeg" : "audio/mp4";
	        streamResult.Stream.Position = 0;
            Response.RegisterForDispose(streamResult);
	        return File(streamResult.Stream, mediaType, false);
        }

        [HttpGet(ApiPaths.GetImage)]
        public async Task<IActionResult> GetImage(string fileName)
        {
            var (streamResult, contentType) = await _songService.GetImage(fileName);
            if (streamResult == null || contentType == null)
                return StatusCode(404);
            Response.RegisterForDispose(streamResult);
            return File(streamResult.Stream, contentType);
        }

        [ApiAuth]
        [HttpPost(ApiPaths.IncrementPlayCount)]
        public async Task<IActionResult> IncrementPlayCount(Incrementer incrementer)
        {
	        await _songService.IncrementPlayCount(incrementer.FileID);
	        return Ok();
        }

        public class Incrementer
        {
            public int FileID { get; set; }
        }

        [ApiAuth]
        [HttpGet(ApiPaths.GetSongListVersion)]
        public async Task<IActionResult> GetSongListVersion()
        {
	        var version = await _songService.GetSongListVersion();
	        return Content(version, "text/plain");
        }
    }
}