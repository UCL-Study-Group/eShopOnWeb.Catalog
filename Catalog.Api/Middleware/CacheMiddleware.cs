using Catalog.Application.Interfaces;

namespace Catalog.Api.Middleware;

public class CacheMiddleware
{
    private readonly RequestDelegate _next;
    
    public  CacheMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICacheService cacheService)
    {
        // Ignores the request, if it is an GET request
        if (context.Request.Method != HttpMethods.Get)
        {
            await _next(context);
            return;
        }
        
        // Let's try to not cache things that should be cached
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;
        if (path.StartsWith("/swagger") || 
            path.StartsWith("/openapi") || 
            path.Contains(".json") ||
            path.Contains(".css") ||
            path.Contains(".js"))
        {
            await _next(context);
            return;
        }

        var cacheKey = GetCacheKey(context.Request);
        
        var cacheResponse = await cacheService.GetCacheAsync(cacheKey);

        if (cacheResponse.IsSuccess)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(cacheResponse.Value);

            return;
        }
        
        var originalBodyStream = context.Response.Body;
        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);

            // Read what the controller returned
            responseBody.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(responseBody).ReadToEndAsync();

            // We wouldn't want to save if it failed, so we check for status code!
            if (context.Response.StatusCode == 200 && !string.IsNullOrEmpty(responseText))
            {
                await cacheService.InsertCacheAsync(cacheKey, responseText);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to insert response into cache: " + ex.Message);
        }
        finally
        {
            // Send the response to the client
            responseBody.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }
        
    }
    
    private static string GetCacheKey(HttpRequest request)
    {
        // The route, so it'll be like api/catalog-item
        var route = request.Path.Value?.ToLowerInvariant();
        
        // The query of the request
        var query = request.QueryString.ToString();
        
        return $"cache:{route}{query}";
    }
}

public static class CacheMiddlewareExtensions
{
    public static IApplicationBuilder UseCacheMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CacheMiddleware>();
    }
}