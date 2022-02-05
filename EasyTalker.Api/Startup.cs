using System.IO;
using EasyTalker.Api.Extensions;
using EasyTalker.Authentication.Extensions;
using EasyTalker.Database.Extensions;
using EasyTalker.Infrastructure.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyTalker.Api;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        var configuration = GetConfiguration();
        
        services.AddAppOptions();
        services.AddAppCors();
        services.AddSignalRCommunication();
        services.AddDatabase(configuration);
        services.AddAppAuthentication(configuration);
        services.AddSwagger();
        services.AddCors();
        services.AddFilePersistenceManager();
        services.AddControllers().AddControllersAsServices();
    }

    private static IConfigurationRoot GetConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(Application.AppSettingsFile)
            .Build();
    }
}