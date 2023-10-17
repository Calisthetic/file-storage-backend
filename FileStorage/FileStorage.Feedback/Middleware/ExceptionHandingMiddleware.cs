using FileStorage.Feedback.Models.Outcoming;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace FileStorage.Feedback.Middleware
{
    public class ExceptionHandingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandingMiddleware> _logger;

        public ExceptionHandingMiddleware(RequestDelegate next, ILogger<ExceptionHandingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (DbUpdateException ex)
            {
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.InternalServerError, "Database saving error");
            }
            catch (DivideByZeroException ex)
            {
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.InternalServerError, "Divide by zero");
            }
            catch (ValidationException ex)
            {
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.BadRequest, "Input value is not valid");
            }
            catch (NullReferenceException ex)
            {
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.InternalServerError, "Get nulls...");
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex, HttpStatusCode.InternalServerError, "Internal server error");
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex, HttpStatusCode httpStatusCode, string message)
        {
            _logger.LogError(ex.InnerException?.Message ?? ex.Message);

            HttpResponse response = context.Response;

            response.ContentType = "application/json";
            response.StatusCode = (int)httpStatusCode;

            ErrorDto badResponse = new ErrorDto()
            {
                Message = message,
                Exception = ex.Message //ex.GetType().Name
            };

            string result = JsonSerializer.Serialize(badResponse);

            await response.WriteAsync(result);
        }
    }
}
