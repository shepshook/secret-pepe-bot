using Telegram.Bot.Types;

namespace SecretPepeBot.Client.Abstractions;

public interface ITelegramUpdateHandler
{
    Task HandleUpdateAsync(Update update, CancellationToken ct);
}
