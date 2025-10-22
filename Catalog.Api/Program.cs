using Catalog.Api.Middleware;
using Catalog.Application;
using Catalog.Infrastructure;
using Microsoft.OpenApi.Models;

namespace Catalog.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        
        builder.Services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Servers = new List<OpenApiServer>
                {
                    new() { Url = "http://localhost:8089", Description = "Local Docker API" }
                };
                return Task.CompletedTask;
            });
        });
        
        builder.Services.AddEndpointsApiExplorer();
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddApplication(builder.Configuration);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "eShopOnWeb Catalog API v1");
            options.RoutePrefix = string.Empty;
        });

        app.UseCors("AllowAll");
        app.UseCacheMiddleware();
        
        //app.UseHttpsRedirection();
        app.UseAuthorization();
        
        app.MapControllers();

        app.Run();
    }
}