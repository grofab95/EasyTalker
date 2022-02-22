using EasyTalker.Api;
using EasyTalker.Api.Extensions;
using EasyTalker.Database;
using EasyTalker.Database.Entities;
using Serilog;

Log.Logger = EasyTalker.Infrastructure.Logger.LoggerFactory.Create();
//FakeData();
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
Startup.ConfigureServices(builder.Services);
builder
    .Build()
    .ConfigureService()
    .Run();


void FakeData()
{
    var users = Enumerable.Range(1, 20)
        .Select(x => new UserDb
        {
            IsActive = true,
            Email = Faker.Internet.Email(),
            UserName = Faker.Internet.UserName() 
        });

   
    var ctx = new EasyTalkerContext();
    
    ctx.Users.AddRange(users);
    ctx.SaveChanges();
}