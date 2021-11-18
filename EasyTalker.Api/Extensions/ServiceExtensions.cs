using System;
using System.Security.Claims;
using System.Threading.Tasks;
using EasyTalker.Api.Authentication.Handlers;
using EasyTalker.Api.Authentication.Services;
using EasyTalker.Database;
using EasyTalker.Database.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.VisualBasic;
using TokenHandler = EasyTalker.Api.Authentication.Handlers.TokenHandler;

namespace EasyTalker.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddEasyTalkerAuthentication(this IServiceCollection services)
        {
            services.AddIdentity<UserDb, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 2;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
            .AddEntityFrameworkStores<EasyTalkerContext>()
            .AddDefaultTokenProviders();

            services.AddScoped<ITokenHandler, TokenHandler>();
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
                        ClockSkew = TimeSpan.FromSeconds(30)
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            var path = context.HttpContext.Request.Path;

                            if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/uiHub") || path.StartsWithSegments("/terminalHub")))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });
            
            services.AddAuthorization(options =>
            {
                // foreach (var permission in Permissions.All)
                // {
                //     options.AddPolicy(permission, policyBuilder =>
                //     {
                //         policyBuilder.RequireAuthenticatedUser();
                //         policyBuilder.RequireClaim(ApplicationClaimTypes.Permission, permission);
                //     });
                // }
                //
                // options.AddPolicy(Permissions.GetTermialsList, policyBuilder =>
                // {
                //     policyBuilder.RequireAssertion(context =>
                //         context.User.HasClaim(c => c.Value is Permissions.ViewTerminalList or Permissions.AddTerminal or Permissions.EditTerminal or Permissions.EditGroup or Permissions.EditShop));
                // });
                //
                // options.AddPolicy(Permissions.GetGroupsList, policyBuilder =>
                // {
                //     policyBuilder.RequireAssertion(context =>
                //         context.User.HasClaim(c => c.Value is Permissions.ViewGroupList or Permissions.AddGroup or Permissions.EditGroup or Permissions.ViewTerminalList or Permissions.EditTerminal));
                // });
            });
        }
        
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
    }
}