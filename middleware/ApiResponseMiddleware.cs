using System.Text.Json;

namespace FileUploadSvc.middleware;

 public class ApiResponseMiddleware
{
    private readonly RequestDelegate _next;

    public ApiResponseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;

        using (var newBodyStream = new MemoryStream())
        {
            context.Response.Body = newBodyStream;

            try
            {
                await _next(context);

                context.Response.Body = originalBodyStream;

                if (context.Response.ContentType != null &&  !(
                    context.Response.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase) || 
                    context.Response.ContentType.StartsWith("application/problem+json", StringComparison.OrdinalIgnoreCase) 
                    ))
                {
                    newBodyStream.Seek(0, SeekOrigin.Begin);
                    await newBodyStream.CopyToAsync(originalBodyStream);
                    return;
                }


                var statusCode = context.Response.StatusCode;
                var status = statusCode >= 200 && statusCode < 300 ? "success" : "error";
                var message = GetDefaultMessageForStatusCode(statusCode);

                newBodyStream.Seek(0, SeekOrigin.Begin);
                var responseBody = new StreamReader(newBodyStream).ReadToEnd();
                var wrappedResponse = JsonSerializer.Serialize(new
                {
                    code = statusCode,
                    status,
                    message,
                    data = string.IsNullOrWhiteSpace(responseBody) ? null : JsonSerializer.Deserialize<object>(responseBody)
                });

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(wrappedResponse);
            }
            catch (Exception ex)
            {
                context.Response.Body = originalBodyStream;

                var wrappedResponse = JsonSerializer.Serialize(new
                {
                    code = 500,
                    status = "error",
                    message = "An unhandled exception occurred",
                    data = (object)null
                });

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(wrappedResponse);
            }
        }
    }

    private string GetDefaultMessageForStatusCode(int statusCode)
    {
        return statusCode switch
        {
            200 => "OK",
            201 => "Created",
            204 => "No Content",
            400 => "Bad Request",
            401 => "Unauthorized",
            403 => "Forbidden",
            404 => "Not Found",
            500 => "Internal Server Error",
            _ => "An error occurred"
        };
    }
}