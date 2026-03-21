using Discord;
using Discord.WebSocket;
using DiscordBot.Config;

namespace DiscordBot.Services;

public class DiscordLogService
{
    private const string LogChannelPath = "logs/logchannel.txt";

    private readonly DiscordSocketClient _client;
    private readonly ulong _logChannelId;

    public DiscordLogService(DiscordSocketClient client, BotConfig config)
    {
        _client = client;
        _logChannelId = config.LogChannelId;
    }

    private ulong GetLogChannelId()
    {
        if (File.Exists(LogChannelPath))
        {
            var text = File.ReadAllText(LogChannelPath).Trim();
            if (ulong.TryParse(text, out var id))
                return id;
        }
        return _logChannelId;
    }

    public async Task SendDeleteLogAsync(
        IMessage msg, string channelName,
        string deletedBy, int totalDeleted)
    {
        if (_client.GetChannel(GetLogChannelId()) is not ITextChannel ch) return;

        var embed = new EmbedBuilder()
            .WithTitle("🗑️ Mensaje borrado")
            .WithColor(Color.Red)
            .AddField("👤 Autor", $"{msg.Author.Username} (`{msg.Author.Id}`)", inline: true)
            .AddField("📢 Canal", $"#{channelName}", inline: true)
            .AddField("🛡️ Borrado por", deletedBy, inline: true)
            .AddField("💬 Contenido",
                string.IsNullOrWhiteSpace(msg.Content)
                    ? "_[embed / sin texto]_"
                    : $"```{msg.Content}```")
            .AddField("📦 Total", $"{totalDeleted} mensajes", inline: true)
            .WithTimestamp(DateTimeOffset.UtcNow)
            .WithFooter("Log de moderación")
            .Build();

        await ch.SendMessageAsync(embed: embed);
    }

    public async Task SendBulkDeleteSummaryAsync(
        string channelName, int count, string deletedBy)
    {
        if (_client.GetChannel(GetLogChannelId()) is not ITextChannel ch) return;

        var embed = new EmbedBuilder()
            .WithTitle("🗑️ Borrado masivo completado")
            .WithColor(Color.DarkRed)
            .AddField("📢 Canal", $"#{channelName}", inline: true)
            .AddField("🛡️ Por", deletedBy, inline: true)
            .AddField("📦 Total", $"{count} mensajes", inline: true)
            .WithTimestamp(DateTimeOffset.UtcNow)
            .Build();

        await ch.SendMessageAsync(embed: embed);
    }
}