using System;
using System.Net;
using Newtonsoft.Json;

namespace LoanServicingApi.Middleware
{
	public class ErrorHandlingMiddleware
	{
		private readonly RequestDelegate _next;

		public ErrorHandlingMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context); //invoke http requests
			}
			catch(Exception ex) //if there is exception it will caught here
			{
				await HandleExceptionAsync(context, ex);
			}
		}

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
			var code = HttpStatusCode.InternalServerError;
			var result = JsonConvert.SerializeObject(new { error = "An error occure while processing your request" });
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)code;
			return context.Response.WriteAsync(result);
        }
    }
}

