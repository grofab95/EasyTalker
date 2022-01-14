using EasyTalker.Database.Entities;
using EasyTalker.Database.Store;
using EasyTalker.Database.Views;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
        optionsBuilder
            .UseSqlServer("Server=DESKTOP-HV06FGL;Database=EasyTalker;User Id=sa; Password=Q1w2e3;");
        
        optionsBuilder.EnableSensitiveDataLogging();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ConversationInfosView>(x =>
        {
            x.HasNoKey();
            x.ToView("ConversationInfosView"); 
        });

        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EasyTalkerContext).Assembly); // Here UseConfiguration is any IEntityTypeConfiguration
    }
}