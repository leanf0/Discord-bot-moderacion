using Discord;
using Discord.Commands;
using DiscordBot.Services;

namespace DiscordBot.Modules;

[RequireContext(ContextType.Guild)]
public class ModerationModule : ModuleBase<SocketCommandContext>
{
    private readonly LogService _logService;
    private readonly DiscordLogService _discordLog;

    public ModerationModule(LogService logService, DiscordLogService discordLog)
    {
        _logService = logService;
        _discordLog = discordLog;
    }

    [Command("clear")]
    [Alias("purge", "delete")]
    [RequireUserPermission(GuildPermission.ManageMessages)]
    [RequireBotPermission(GuildPermission.ManageMessages)]
    public async Task ClearAsync(int amount = 5)
    {
        if (amount < 1 || amount > 100)
        {
            await ReplyAsync("❌ La cantidad debe estar entre 1 y 100.");
            return;
        }

        var messages = (await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync()).ToList();
        var channel = (ITextChannel)Context.Channel;
        var deletedBy = Context.User.Username;
        int total = messages.Count - 1;

        foreach (var msg in messages)
        {
            await _logService.LogDeletedMessageAsync(
                msg.Author.Username, msg.Author.Id.ToString(),
                channel.Name, msg.Content, total, deletedBy);

            await _discordLog.SendDeleteLogAsync(msg, channel.Name, deletedBy, total);
        }

        await channel.DeleteMessagesAsync(messages);
        await _discordLog.SendBulkDeleteSummaryAsync(channel.Name, total, deletedBy);

        var confirm = await ReplyAsync($"✅ Se borraron {total} mensajes.");
        await Task.Delay(4000);
        await confirm.DeleteAsync();
    }

    [Command("log")]
    [RequireUserPermission(GuildPermission.ManageMessages)]
    public async Task ShowLogAsync()
    {
        const string path = "logs/deleted_messages.log";

        if (!File.Exists(path))
        {
            await ReplyAsync("📭 No hay registros todavía.");
            return;
        }

        var lines = (await File.ReadAllLinesAsync(path)).TakeLast(10).ToArray();

        var embed = new EmbedBuilder()
            .WithTitle("📋 Log de mensajes borrados")
            .WithDescription($"```\n{string.Join("\n", lines)}\n```")
            .WithColor(Color.Orange)
            .WithCurrentTimestamp()
            .Build();

        await ReplyAsync(embed: embed);
    }

    [Command("setlog")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task SetLogChannelAsync()
    {
        var path = "logs/logchannel.txt";
        await File.WriteAllTextAsync(path, Context.Channel.Id.ToString());
        await ReplyAsync("✅ Este canal fue configurado como canal de logs.");
    }
}