using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LoanServicingApi.Filters
{
	public class ErrorHandlingFilterAttribute : ExceptionFilterAttribute
	{
        public override void OnException(ExceptionContext context) //if there is an unhandled exception it will come here
        { 
            var exception = context.Exception; //we'll get the exception context here

            var problemDetails = new ProblemDetails
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Title = "An error occurred while processing your request",
                Status = (int)HttpStatusCode.InternalServerError,
            };
            context.Result = new ObjectResult(problemDetails);
            context.ExceptionHandled = true;
        }
    }
}

