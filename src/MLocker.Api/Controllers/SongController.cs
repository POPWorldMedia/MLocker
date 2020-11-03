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

        //[ApiAuth]
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
            var (stream, song) = await _songService.GetSong(id);
            if (stream == null || song == null)
                return StatusCode(404);
            var mediaType = song.FileName.EndsWith("mp3") ? "audio/mpeg" : "audio/mp4";
            return File(stream, mediaType, true);
        }

        [HttpGet(ApiPaths.GetWholeSong + "/{id}")]
        public async Task<IActionResult> GetWholeSong(int id)
        {
	        var (stream, song) = await _songService.GetSong(id);
	        if (stream == null || song == null)
		        return StatusCode(404);
	        var mediaType = song.FileName.EndsWith("mp3") ? "audio/mpeg" : "audio/mp4";
	        var length = (int)stream.Length;
	        var bytes = new byte[length];
	        await stream.ReadAsync(bytes, 0, length);
            return File(bytes, mediaType);
        }

        [HttpGet(ApiPaths.GetImage)]
        public async Task<IActionResult> GetImage(string fileName)
        {
            var (stream, contentType) = await _songService.GetImage(fileName);
            if (stream == null || contentType == null)
                return StatusCode(404);
            return File(stream, contentType);
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

        [HttpGet(ApiPaths.GetSongListVersion)]
        public async Task<IActionResult> GetSongListVersion()
        {
	        var version = await _songService.GetSongListVersion();
	        return Content(version, "text/plain");
        }
    }
}