using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MLocker.Api.Services;

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

        [HttpGet("/GetAllSongs")]
        public async Task<IActionResult> GetAll()
        {
            var allSongs = await _songService.GetAll();
            return Ok(allSongs);
        }

        [HttpGet("/GetSong/{id}")]
        public async Task<IActionResult> GetSong(int id)
        {
            var (stream, song) = await _songService.GetSong(id);
            var mediaType = song.FileName.EndsWith("mp3") ? "audio/mpeg" : "audio/mp4";
            return File(stream, mediaType);
        }
    }
}