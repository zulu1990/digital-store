using Application;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Persistance;
using OnlineStore.Extensions;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();


        builder.Services
            .AddUI()
            .AddApplicationLayer()
            .AddInfrastructure(builder.Configuration);

        var app = builder.Build();

        using(var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ApplicationDbContext>();

            SeedData.SeedProducts(context);
        }


        app.UseMiddleware<ExceptionHandlingMiddleware>();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}