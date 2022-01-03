using EasyTalker.Api.Extensions;
using EasyTalker.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EasyTalker.Database.Extensions;
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

        services.AddControllers();
        services.AddDatabase(_configuration);
        services.AddEasyTalkerAuthentication();
        services.AddSwagger();
        services.AddCors();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EasyTalker API Documentation v1"));
        }

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCors(x => x
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true) // allow any origin
            .AllowCredentials()); // allow credentials
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}