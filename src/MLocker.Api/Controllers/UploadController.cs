using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MLocker.Api.Services;
using MLocker.Core.Models;
using MLocker.Core.Services;

namespace MLocker.Api.Controllers
{
    [NoGuestAuth]
    [ApiAuth]
    [ApiController]
    public class UploadController(IFileParsingService fileParsingService, ISongService songService)
        : ControllerBase
    {
        [HttpPost(ApiPaths.Upload)]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return StatusCode(204);
            var stream = file.OpenReadStream();
            var length = (int)stream.Length;
            var fileData = new byte[length];
            await stream.ReadExactlyAsync(fileData, 0, length);
            var songData = fileParsingService.ReadFileData(file.FileName, fileData);
            await songService.PersistSong(songData, fileData);
            return Ok();
        }
    }
}
