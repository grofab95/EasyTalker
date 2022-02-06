using EasyTalker.Api.Hubs;
using EasyTalker.Core.Adapters;
using EasyTalker.Core.Configuration;
using EasyTalker.Core.Events;
using EasyTalker.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace EasyTalker.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureService(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }
        app.UseCors();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapFallbackToFile("index.html"); 
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<WebUiHub>("/uiHub");
        });

        var pathOptions = app.Services.GetRequiredService<IOptions<PathsOptions>>();
        app.UseStaticFiles(new StaticFileOptions
        {
            RequestPath = "/static",
            FileProvider = new PhysicalFileProvider(pathOptions.Value.UploadFilesPath)
        });

        var eventHandlerCollector = app.Services.GetRequiredService<EventHandlerCollector>();
        eventHandlerCollector.RegisterHandlers();
        
        using var scope = app.Services.CreateScope();
        var userStore = scope.ServiceProvider.GetService<IUserStore>();
        userStore?.SetAllUsersAsOffline()?.Wait();

        var databaseInitializer = scope.ServiceProvider.GetService<DatabaseInitializer>();
        databaseInitializer?.Initialize().Wait();
        
        return app;
    }
}