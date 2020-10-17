using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace MLocker.Api.Controllers
{
	public class ApiAuthAttribute : ActionFilterAttribute, IAuthorizationFilter
	{
		public void OnAuthorization(AuthorizationFilterContext context)
		{
			var config = context.HttpContext.RequestServices.GetService<IConfig>();
			var authHeader = context.HttpContext.Request.Headers["Authorization"];
			if (string.IsNullOrEmpty(authHeader) || authHeader != config.ApiKey)
			{
				context.Result = new UnauthorizedResult();
			}
		}
	}
}