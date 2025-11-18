using System.Net;
using System.Text.Json;
using TeamSync.Application.Common.Exceptions;

namespace TeamSync.API.Middleware
{
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionMiddleware> _logger;

		public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unhandled Exception");
				await HandleExceptionAsync(context, ex);
			}
		}

		private Task HandleExceptionAsync(HttpContext context, Exception ex)
		{
			var response = context.Response;
			response.ContentType = "application/json";

			int statusCode;
			string errorType = ex.GetType().Name;

			statusCode = ex switch
			{
				ValidationException => (int)HttpStatusCode.BadRequest,
				NotFoundException => (int)HttpStatusCode.NotFound,
				UnauthorizedException => (int)HttpStatusCode.Unauthorized,
				ConflictException => (int)HttpStatusCode.Conflict,
				_ => (int)HttpStatusCode.InternalServerError
			};

			response.StatusCode = statusCode;

			var result = JsonSerializer.Serialize(new
			{
				success = false,
				error = errorType,
				message = ex.Message,
				statusCode
			});

			return response.WriteAsync(result);
		}
	}
}
