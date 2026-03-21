namespace DiscordBot.Services;

public class LogService
{
    private const string LogPath = "logs/deleted_messages.log";

    public LogService()
        => Directory.CreateDirectory("logs");

    public async Task LogDeletedMessageAsync(
        string authorName, string authorId,
        string channelName, string content,
        int amount, string deletedBy)
    {
        var entry = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC] " +
                    $"Por: {deletedBy} | Canal: #{channelName} | " +
                    $"Autor: {authorName} ({authorId}) | " +
                    $"Cantidad: {amount} | Contenido: {content}";

        await File.AppendAllTextAsync(LogPath, entry + Environment.NewLine);
    }
}