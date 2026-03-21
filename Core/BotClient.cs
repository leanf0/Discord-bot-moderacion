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
    private DiscordSocketClient _client = null!;
    private CommandService _commands = null!;
    private IServiceProvider _services = null!;
    private BotConfig _config = null!;

    public async Task RunAsync()
    {
        _config = new BotConfig();

        _client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds
                           | GatewayIntents.GuildMessages
                           | GatewayIntents.MessageContent
        });

        _commands = new CommandService(new CommandServiceConfig
        {
            CaseSensitiveCommands = false,
            DefaultRunMode = RunMode.Async
        });

        _services = new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_commands)
            .AddSingleton(_config)
            .AddSingleton<LogService>()
            .AddSingleton<DiscordLogService>()
            .BuildServiceProvider();

        _client.Log += OnLog;
        _client.Ready += OnReady;
        _client.MessageReceived += OnMessageReceived;

        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        await _client.LoginAsync(TokenType.Bot, _config.Token);
        await _client.StartAsync();
        await Task.Delay(Timeout.Infinite);
    }

    private Task OnLog(LogMessage log)
    {
        Console.WriteLine($"[{log.Severity}] {log.Message}");
        return Task.CompletedTask;
    }

    private Task OnReady()
    {
        Console.WriteLine($"✅ Conectado como {_client.CurrentUser.Username}");
        return Task.CompletedTask;
    }

    private async Task OnMessageReceived(SocketMessage rawMessage)
    {
        if (rawMessage is not SocketUserMessage message || message.Author.IsBot) return;

        int argPos = 0;
        if (!message.HasCharPrefix(_config.Prefix, ref argPos)) return;

        var context = new SocketCommandContext(_client, message);
        var result = await _commands.ExecuteAsync(context, argPos, _services);

        if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
            await context.Channel.SendMessageAsync($"❌ {result.ErrorReason}");
    }
}