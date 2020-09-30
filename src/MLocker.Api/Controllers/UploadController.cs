using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MLocker.Core.Services;

namespace MLocker.Api.Controllers
{
    [ApiController]
    public class UploadController : ControllerBase
    {
        [HttpPost("/upload")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return StatusCode(204);
            var fileParser = new FileParsingService();
            var stream = file.OpenReadStream();
            var length = (int)stream.Length;
            var fileData = new byte[length];
            await stream.ReadAsync(fileData, 0, length);
            //System.IO.File.Create(@"C:\Users\jeffy\Desktop\test\" + file.FileName, Convert.ToInt32(file.Length)).Write(fileData);
            var songData = fileParser.ReadFileData(file.Name, fileData);
            // persist blog
            // persist songdata
            return Ok();
        }
    }
}
