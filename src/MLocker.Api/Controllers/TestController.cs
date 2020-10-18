using Microsoft.AspNetCore.Mvc;
using MLocker.Core.Models;

namespace MLocker.Api.Controllers
{
	public class TestController : Controller
	{
		[ApiAuth]
		[HttpGet(ApiPaths.Test)]
		public IActionResult Index()
		{
			return Ok();
		}
	}
}