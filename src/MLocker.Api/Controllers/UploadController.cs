using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MLocker.Api.Services;
using MLocker.Core.Models;
using MLocker.Core.Services;

namespace MLocker.Api.Controllers
{
    [ApiAuth]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IFileParsingService _fileParsingService;
        private readonly ISongService _songService;

        public UploadController(IFileParsingService fileParsingService, ISongService songService)
        {
            _fileParsingService = fileParsingService;
            _songService = songService;
        }

        [HttpPost(ApiPaths.Upload)]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return StatusCode(204);
            var stream = file.OpenReadStream();
            var length = (int)stream.Length;
            var fileData = new byte[length];
            await stream.ReadAsync(fileData, 0, length);
            var songData = _fileParsingService.ReadFileData(file.FileName, fileData);
            await _songService.PersistSong(songData, fileData);
            return Ok();
        }
    }
}
