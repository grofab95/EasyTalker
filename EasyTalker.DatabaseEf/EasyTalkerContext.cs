using System.IO;
using EasyTalker.Database.Entities;
using EasyTalker.Database.Views;
using EasyTalker.Infrastructure.Constants;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EasyTalker.Database;

public class EasyTalkerContext : IdentityDbContext<UserDb>
{
    public DbSet<ConversationDb> Conversations { get; set; }
    public DbSet<MessageDb> Messages { get; set; }
    public DbSet<UserConversationDb> UsersConversations { get; set; }
    public DbSet<RefreshTokenDb> RefreshTokens { get; set; }
    public DbSet<FileDb> Files { get; set; }
    public DbSet<ConversationInfosView> ConversationInfosView { get; set; }

    public EasyTalkerContext()
    {
                
    }
        
    public EasyTalkerContext(DbContextOptions<EasyTalkerContext> options) : base(options)
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

        modelBuilder.Entity<ConversationInfosView>(x =>
        {
            x.HasNoKey();
            x.ToView("ConversationInfosView"); 
        });
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EasyTalkerContext).Assembly); 
    }
}