using EasyTalker.Api;
using EasyTalker.Infrastructure.Logger;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

Log.Logger = LoggerFactory.Create();
        
CreateHostBuilder(args).Build().Run();

IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());