using EasyTalker.Authentication.Database.Entities;
using EasyTalker.Core.Constants;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EasyTalker.Authentication.Database;

public class EasyTalkerAuthenticationContext : IdentityDbContext<UserDb>
{
    public DbSet<RefreshTokenDb> RefreshTokens { get; set; }
    
    
    public EasyTalkerAuthenticationContext()
    {
                
    }
        
    public EasyTalkerAuthenticationContext(DbContextOptions<EasyTalkerAuthenticationContext> options) : base(options)
    {
            
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured) 
            return;

        var appDirectory =
            Path.Combine(new DirectoryInfo(Directory.GetCurrentDirectory())?.Parent?.FullName ?? "", "EasyTalker");
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(appDirectory)
            .AddJsonFile(Application.AppSettingsFile)
            .Build();
        
        var connectionString = configuration.GetConnectionString("Database");
        optionsBuilder.UseSqlServer(connectionString);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EasyTalkerAuthenticationContext).Assembly); 
    }
}