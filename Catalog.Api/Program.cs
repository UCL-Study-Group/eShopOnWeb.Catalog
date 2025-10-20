using Catalog.Application;
using Catalog.Application.Services;
using Catalog.Common.Models;
using Catalog.Infrastructure;
using Catalog.Infrastructure.Models;
using Catalog.Infrastructure.Repositories;

namespace Catalog.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddInfrastructure();
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

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}