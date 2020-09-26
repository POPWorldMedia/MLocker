using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MLocker.Core.Models;

namespace MLocker.Functions
{
    public static class Upload
    {
        [FunctionName("Upload")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequestMessage request, ILogger log)
        {
            log.LogInformation("Upload triggered");

            var payload = await request.Content.ReadAsStringAsync();
            var container = JsonSerializer.Deserialize<UploadContainer>(payload);
            log.LogInformation($"Received payload: {container.Song.FileName}");

            return new OkObjectResult("OK");
        }
    }
}