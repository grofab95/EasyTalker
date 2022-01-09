using System.Text.Json.Serialization;
using Easy.MessageHub;
using EasyTalker.Api.Extensions;
using EasyTalker.Api.Hubs;
using EasyTalker.Core.Adapters;
using EasyTalker.Core.EventHandlers;
using EasyTalker.Core.Utils;
using EasyTalker.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EasyTalker.Database.Extensions;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;

namespace EasyTalker.Api;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        var test = configuration.GetConnectionString("ConnectionString");
            
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(x => x.AddDefaultPolicy(new CorsPolicyBuilder()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(x => true)
            .Build()));
        
        services.AddSignalR()
            .AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        
        services.AddSingleton<IMessageHub, MessageHub>();
        services.AddTransient<EventHandlerCollector>();
        services.AddTransient<IEventHandler, ConversationsEventHandler>();
        services.AddTransient<IEventHandler, UsersEventHandler>();
        
        services.AddSignalR();
        services.AddControllers().AddControllersAsServices();

        services.AddTransient<IWebUiNotifier, WebUiNotifier>();
        services.AddDatabase(_configuration);
        services.AddEasyTalkerAuthentication();
        services.AddSwagger();
        services.AddCors();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, EventHandlerCollector eventHandlerCollector)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EasyTalker API Documentation v1"));
        }

        app.UseCors();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        // app.UseCors(x => x
        //     .AllowAnyMethod()
        //     .AllowAnyHeader()
        //     .SetIsOriginAllowed(origin => true) // allow any origin
        //     .AllowCredentials()); // allow credentials
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<WebUiHub>("/uiHub");
        });
        
        eventHandlerCollector.RegisterHandlers();
        
        app.UseSpa(spa =>
        {
            spa.Options.SourcePath = "ClientApp";

            if (env.IsDevelopment())
            {
                spa.UseReactDevelopmentServer(npmScript: "start");
            }
        });
    }
}