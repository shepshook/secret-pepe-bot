using Microsoft.Extensions.Options;
using SecretPepeBot.Admin.Abstractions;
using Telegram.Bot;

namespace SecretPepeBot.Admin;

public class AdminNotificationService : IAdminNotificationService
{
    private readonly IAdminTelegramBotClient _adminClient;
    private readonly BotOptions _options;

    public AdminNotificationService(IAdminTelegramBotClient adminClient, IOptions<BotOptions> options)
    {
        _adminClient = adminClient;
        _options = options.Value;
    }

    public Task Notify(Notification notification, CancellationToken ct)
    {
        return _adminClient.SendTextMessageAsync(_options.AdminChatId, notification.Message, cancellationToken: ct);
    }
}
