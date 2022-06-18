using EasyTalker.Api;
using EasyTalker.Api.Extensions;
using Serilog;
using LoggerFactory = EasyTalker.Core.Logger.LoggerFactory;

Log.Logger = LoggerFactory.Create();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
Startup.ConfigureServices(builder.Services);

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));


var app = builder.Build();


app.UseHttpsRedirection();
    
app.ConfigureService().Run();