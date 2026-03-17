using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Config;
using DiscordBot.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace DiscordBot.Core;

public class BotClient
{
    public async Task RunAsync()
    {
        var botConfig = new BotConfig();

        var clientConfig = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds
                           | GatewayIntents.GuildMessages
                           | GatewayIntents.MessageContent
        };

        var client = new DiscordSocketClient(clientConfig);
        var commands = new CommandService(new CommandServiceConfig
        {
            CaseSensitiveCommands = false,
            DefaultRunMode = RunMode.Async
        });

        var services = new ServiceCollection()
            .AddSingleton(client)
            .AddSingleton(commands)
            .AddSingleton(botConfig)
            .AddSingleton<LogService>()
            .AddSingleton<DiscordLogService>()
            .BuildServiceProvider();

        client.Log += log =>
        {
            Console.WriteLine($"[{log.Severity}] {log.Message}");
            return Task.CompletedTask;
        };

        client.MessageReceived += async rawMessage =>
        {
            if (rawMessage is not SocketUserMessage message || message.Author.IsBot) return;
            int argPos = 0;
            if (!message.HasCharPrefix(botConfig.Prefix, ref argPos)) return;
            var context = new SocketCommandContext(client, message);
            var result = await commands.ExecuteAsync(context, argPos, services);
            if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                await context.Channel.SendMessageAsync($"❌ {result.ErrorReason}");
        };

        client.Ready += () =>
        {
            Console.WriteLine($"✅ Conectado como {client.CurrentUser.Username}");
            return Task.CompletedTask;
        };

        await commands.AddModulesAsync(Assembly.GetEntryAssembly(), services);
        await client.LoginAsync(TokenType.Bot, botConfig.Token);
        await client.StartAsync();
        await Task.Delay(Timeout.Infinite);
    }
}