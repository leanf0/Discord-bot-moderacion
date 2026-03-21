namespace DiscordBot.Config;

public class BotConfig
{
    public string Token { get; init; }
    public char Prefix { get; init; }
    public ulong LogChannelId { get; init; }

    public BotConfig()
    {
        Token = Environment.GetEnvironmentVariable("DISCORD_TOKEN")
            ?? throw new InvalidOperationException("La variable de entorno DISCORD_TOKEN no está definida.");

        Prefix = Environment.GetEnvironmentVariable("DISCORD_PREFIX")?[0] ?? '$';

        LogChannelId = ulong.TryParse(
            Environment.GetEnvironmentVariable("LOG_CHANNEL_ID"), out var id) ? id : 0;
    }
}