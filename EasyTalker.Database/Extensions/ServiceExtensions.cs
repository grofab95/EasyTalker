using EasyTalker.Core.Adapters;
using EasyTalker.Database.Mapper;
using EasyTalker.Database.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyTalker.Database.Extensions;

public static class ServiceExtensions
{
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<EasyTalkerContext>();
        services.AddAutoMapper(typeof(AutoMapperProfile));
        services.AddTransient<IUserStore, UserStore>();
        //services.AddDbContext<EasyTalkerContext>(options =>
        //options.UseSqlServer(configuration.GetConnectionString("ConnectionString")));
    }
}