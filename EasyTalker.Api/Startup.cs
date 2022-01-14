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
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.IO;
using EasyTalker.Core.Configuration;
using EasyTalker.Core.Files;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;

namespace EasyTalker.Api;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOptions<PathsOptions>()
            .Configure<IConfiguration>(
                (o, c) => c.GetSection(PathsOptions.SectionName).Bind(o));
        
        services.AddControllersWithViews();
        services.AddCors(x => x.AddDefaultPolicy(new CorsPolicyBuilder()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true)
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
        services.AddTransient<FilePersistenceManager>();

        services.AddEasyTalkerAuthentication();
        services.AddSwagger();
        services.AddCors();
        
        // services.Configure<FormOptions>(o =>  // currently all set to max, configure it to your needs!
        // {
        //     o.ValueLengthLimit = int.MaxValue;
        //     o.MultipartBodyLengthLimit = long.MaxValue; // <-- !!! long.MaxValue
        //     o.MultipartBoundaryLengthLimit = int.MaxValue;
        //     o.MultipartHeadersCountLimit = int.MaxValue;
        //     o.MultipartHeadersLengthLimit = int.MaxValue;
        // });
        
        // services.AddSpaStaticFiles(configuration =>
        // {
        //     configuration.RootPath = "ClientApp/build";
        // });
        
        // services.Configure<StaticFileOptions>(options =>
        // {
        //     options.FileProvider = new PhysicalFileProvider("I://Dev//UploadedFiles//");
        //     options.RequestPath = "/static";
        // });
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
        // app.UseStaticFiles(new StaticFileOptions
        // {
        //     RequestPath = "/static",
        //     FileProvider = new PhysicalFileProvider(@"I:\Dev\UploadedFiles\"),
        // });

        // app.UseStaticFiles(new StaticFileOptions
        // {
        //     RequestPath = string.Empty,  // default behavior: static files from "/" (root)
        //     FileProvider = new PhysicalFileProvider(env.WebRootPath),
        // });
        //app.UseSpaStaticFiles();
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
               
        // app.UseSpa(spa =>
        // {
        //     spa.Options.SourcePath = "ClientApp";
        //
        //     // if (env.IsDevelopment())
        //     // {
        //     //     spa.UseReactDevelopmentServer(npmScript: "start");
        //     // }
        // });
        //
        // app.MapWhen(x => !x.Request.Path.Value.StartsWith("/api"), builder =>
        // {
        //     app.Run(async (context) =>
        //     {
        //         context.Response.ContentType = "text/html";
        //         context.Response.Headers[HeaderNames.CacheControl] = "no-store, no-cache, must-revalidate";
        //         await context.Response.SendFileAsync(Path.Combine(env.WebRootPath, "index.html"));
        //     });
        // });
    }
}