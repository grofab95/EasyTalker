using System;
using System.Text.Json.Serialization;
using Easy.MessageHub;
using EasyTalker.Api.Hubs;
using EasyTalker.Core.Adapters;
using EasyTalker.Core.Configuration;
using EasyTalker.Core.EventHandlers;
using EasyTalker.Core.Events;
using EasyTalker.Core.Files;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace EasyTalker.Api.Extensions;

public static class ServiceExtensions
{  
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo 
            { 
                Title = "EasyTalker Api Documentation", 
                Version = "v1", 
                Contact = new OpenApiContact
                {
                    Name = "Fabian Grochla",
                    Email = "fabian.grochla@gmail.com"
                }
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    public static void AddAppOptions(this IServiceCollection services)
    {
        services.AddOptions<PathsOptions>()
            .Configure<IConfiguration>(
                (o, c) => c.GetSection(PathsOptions.SectionName).Bind(o));
    }

    public static void AddAppCors(this IServiceCollection services)
    {
        services.AddCors(x => x.AddDefaultPolicy(new CorsPolicyBuilder()
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true)
            .Build()));
    }

    public static void AddSignalRCommunication(this IServiceCollection services)
    {
        services.AddSignalR()
            .AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        
        services.AddSingleton<IMessageHub, MessageHub>();
        services.AddTransient<EventHandlerCollector>();
        services.AddTransient<IEventHandler, ConversationsEventHandler>();
        services.AddTransient<IEventHandler, UsersEventHandler>();
        services.AddTransient<IEventHandler, FilesEventHandler>();
        services.AddTransient<IWebUiNotifier, WebUiNotifier>();
    }

    public static void AddFilePersistenceManager(this IServiceCollection services)
    {
        services.AddTransient<FilePersistenceManager>();
    }
}