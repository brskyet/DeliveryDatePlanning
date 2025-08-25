using System.Net;
using System.Text.Json;
using DeliveryDatePlanning.Application.Common.Exception;
using DeliveryDatePlanning.WebApi.Model;

namespace DeliveryDatePlanning.WebApi.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionHandlerMiddleware> logger;

    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        this.next = next ?? throw new ArgumentNullException(nameof(next));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await this.next(context);
        }
        catch (Exception exception)
        {
            this.logger.LogError("An unhandled error occurred while app handle request {Exception}", exception);
            
            await HandleExceptionAsync(context, exception);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var result = string.Empty;
        
        switch(exception)
        {
            case ValidationException:
                code = HttpStatusCode.BadRequest;

                break;
            
            case NotFoundException:
                code = HttpStatusCode.NotFound;
                
                break;
            
            case InternalException:
                code = HttpStatusCode.InternalServerError;
                
                break;
        }
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        if (result == string.Empty)
        {
            result = JsonSerializer.Serialize(new ErrorResponse { Error = exception.Message });
        }

        return context.Response.WriteAsync(result);
    }
}