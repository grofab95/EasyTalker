using EasyTalker.Core.Adapters;
using EasyTalker.Database.Dao;
using EasyTalker.Database.Mapper;
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
        services.AddTransient<IUserDao, UserDao>();
        //services.AddDbContext<EasyTalkerContext>(options =>
        //options.UseSqlServer(configuration.GetConnectionString("ConnectionString")));
    }
}