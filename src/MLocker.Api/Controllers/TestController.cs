using Microsoft.AspNetCore.Mvc;

namespace MLocker.Api.Controllers
{
	public class TestController : Controller
	{
		[ApiAuth]
		[HttpGet("/test")]
		public IActionResult Index()
		{
			return Ok();
		}
	}
}