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
        var connectionString = configuration.GetConnectionString("Database");
        services.AddDbContext<EasyTalkerContext>(o => o.UseSqlServer(connectionString));
        services.AddAutoMapper(typeof(AutoMapperProfile));
        services.AddTransient<IUserStore, UserStore>();
        services.AddTransient<IConversationStore, ConversationStore>();
        services.AddTransient<IMessageStore, MessageStore>();
        services.AddTransient<IFileStore, FileStore>();
        services.AddTransient<DatabaseInitializer>();
    }
}