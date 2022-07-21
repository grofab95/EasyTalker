using EasyTalker.Authentication.Configuration;
using EasyTalker.Authentication.Database;
using EasyTalker.Authentication.Database.Entities;
using EasyTalker.Authentication.Database.Store;
using EasyTalker.Authentication.Handlers;
using EasyTalker.Authentication.Services;
using EasyTalker.Core.Adapters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EasyTalker.Authentication.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddAppAuthentication(this IServiceCollection services, IConfigurationRoot configuration)
    {
        var authenticationOption = configuration
            .GetSection(AuthenticationOption.SectionKey)
            .Get<AuthenticationOption>();
        
        var passOptions = authenticationOption.Password;
        var connectionString = configuration.GetConnectionString("Database");
        
        services.AddDbContext<EasyTalkerAuthenticationContext>(
            o => o.UseSqlServer(connectionString));
        
        services.AddIdentity<UserDb, IdentityRole>(options => 
        {
            options.Password.RequiredLength = passOptions.RequiredLength;
            options.Password.RequireDigit = passOptions.RequireDigit;
            options.Password.RequireLowercase = passOptions.RequireLowercase;
            options.Password.RequireUppercase = passOptions.RequireUppercase;
            options.Password.RequireNonAlphanumeric = passOptions.RequireNonAlphanumeric;
        })
        .AddEntityFrameworkStores<EasyTalkerAuthenticationContext>()
        .AddDefaultTokenProviders(); 
        
        services.AddScoped<ITokenHandler, Handlers.TokenHandler>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Constants.Authentication.JwtBearer.Issuer,
                    ValidAudience = Constants.Authentication.JwtBearer.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String("ZGZnaGRmZ2VkeWVydHlSRGhkZnUzZTQ2NTM0NjVnNDM1djY0NWJ2d3ZiZHh2")),
                    ClockSkew = TimeSpan.FromMinutes(30)
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/uiHub"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        
        services.AddTransient<IUserStore, UserStore>();
    }
}