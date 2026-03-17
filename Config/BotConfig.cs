using Microsoft.Extensions.Configuration;

namespace DiscordBot.Config;

public class BotConfig
{
    public string Token { get; init; }
    public char Prefix { get; init; }
    public ulong LogChannelId { get; init; }

    public BotConfig()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        Token = config["Bot:Token"]
            ?? throw new Exception("Token no encontrado en appsettings.json");

        Prefix = config["Bot:Prefix"]?[0] ?? '$';

        LogChannelId = ulong.TryParse(config["Bot:LogChannelId"], out var id)
            ? id
            : throw new Exception("LogChannelId inválido en appsettings.json");
    }
}