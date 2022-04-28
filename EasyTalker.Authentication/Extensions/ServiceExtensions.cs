using EasyTalker.Authentication.Database;
using EasyTalker.Authentication.Database.Entities;
using EasyTalker.Authentication.Database.Mapper;
using EasyTalker.Authentication.Database.Store;
using EasyTalker.Authentication.Handlers;
using EasyTalker.Authentication.Options;
using EasyTalker.Authentication.Services;
using EasyTalker.Core.Adapters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EasyTalker.Authentication.Extensions;

public static class ServiceExtensions
{
    public static void AddAppAuthentication(this IServiceCollection services, IConfigurationRoot configuration)
    {
        var authenticationOption = configuration.GetSection(AuthenticationOption.SectionKey).Get<AuthenticationOption>();
        
        services.AddAutoMapper(typeof(AutoMapperProfile));
        services.AddDbContext<EasyTalkerAuthenticationContext>(o => o.UseSqlServer("Server=DESKTOP-HV06FGL;Database=EasyTalker-Dapper;User Id=sa; Password=Q1w2e3Q1w2e3;"));
        services.AddIdentity<UserDb, IdentityRole>(options => 
            {
                options.Password.RequiredLength = authenticationOption.Password.RequiredLength;
                options.Password.RequireDigit = authenticationOption.Password.RequireDigit;
                options.Password.RequireLowercase = authenticationOption.Password.RequireLowercase;
                options.Password.RequireUppercase = authenticationOption.Password.RequireUppercase;
                options.Password.RequireNonAlphanumeric = authenticationOption.Password.RequireNonAlphanumeric;
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