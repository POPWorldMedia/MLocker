using System.Linq;
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
			if (HttpContext.Items.ContainsKey("IsGuest"))
			{
				HttpContext.Response.Headers["X-Is-Guest"] = "true";
				return Ok("Guest!");
			}
			return Ok();
		}
	}
}