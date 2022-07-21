using Serilog;
using Serilog.Events;
using Serilog.Extensions.Hosting;

namespace EasyTalker.Core.Logger;

public static class LoggerFactory
{
    public static ReloadableLogger Create()
    {
        return new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
                "logs//LOG_.log",
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Information,
                rollingInterval: RollingInterval.Day)
            .CreateBootstrapLogger();
    }
}