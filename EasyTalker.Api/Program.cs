using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyTalker.Database;
using EasyTalker.Database.Entities;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace EasyTalker.Api;

public static class Program
{
    private static void Set()
    {
        var ctx = new EasyTalkerContext();
        var users = ctx.Users.ToList();

        var conversation = new ConversationDb
        {
            Title = "Powitalna"
        };
        
        ctx.Conversations.Add(conversation);
        ctx.SaveChanges();

        var userConversations = users
            .Select(x => new UserConversationDb(x.Id, conversation.Id))
            .ToArray();
        
        ctx.UsersConversations.AddRange(userConversations);

        var messages = new List<MessageDb>
        {
            new MessageDb
            {
                ConversationId = conversation.Id,
                Text = "Witam serdecznie",
                SenderId = users[0].Id
            },
            
            new MessageDb
            {
                ConversationId = conversation.Id,
                Text = "Cześć",
                SenderId = users[1].Id
            }};

        ctx.Messages.AddRange(messages);
        ctx.SaveChanges();
    }
    
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(
                "logs//LOG_.log",
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Information,
                rollingInterval: RollingInterval.Day)
            .CreateBootstrapLogger();
        
        //Set();
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}