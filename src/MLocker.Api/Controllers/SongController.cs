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
    }
}