using EasyTalker.Database.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EasyTalker.Database
{
    public class EasyTalkerContext : IdentityDbContext<UserDb>
    {

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
    }
}