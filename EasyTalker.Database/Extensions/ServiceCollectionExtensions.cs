using System.Data;
using EasyTalker.Core.Adapters;
using EasyTalker.Database.Store;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyTalker.Database.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        
        services.AddTransient<IDbConnection>(_ => new SqlConnection(connectionString));
        services.AddTransient<IConversationStore, ConversationStore>();
        services.AddTransient<IMessageStore, MessageStore>();
        services.AddTransient<IFileStore, FileStore>();
    }
}