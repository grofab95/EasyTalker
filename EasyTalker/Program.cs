using EasyTalker.Api;
using EasyTalker.Api.Extensions;
using Serilog;

Log.Logger = EasyTalker.Infrastructure.Logger.LoggerFactory.Create();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
Startup.ConfigureServices(builder.Services);
builder
    .Build()
    .ConfigureService()
    .Run();
