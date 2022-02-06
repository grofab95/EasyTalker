using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace EasyTalker.Database;

public class DatabaseInitializer
{
    private readonly EasyTalkerContext _easyTalkerContext;

    public DatabaseInitializer(EasyTalkerContext easyTalkerContext)
    {
        _easyTalkerContext = easyTalkerContext;
    }

    public async Task Initialize()
    {
        Log.Information("{DatabaseInitializer} | Migrating database", nameof(DatabaseInitializer));
        await _easyTalkerContext.Database.MigrateAsync();
    }
}