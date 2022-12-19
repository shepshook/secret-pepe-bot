using SecretPepeBot.Admin;
using SecretPepeBot.Admin.Abstractions;
using SecretPepeBot.Client.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SecretPepeBot.Client.States;

public class ParticipantRegisteredState : ClientStateBase
{
    public const string StateName = "registered";
    public override string Name => StateName;

    private readonly ISantaService _santaService;
    private readonly IAdminNotificationService _notificationService;
    private readonly ITelegramBotClient _client;

    protected override Dictionary<string, (HandleDelegate Handler, string NextState)> Commands { get; }

    public ParticipantRegisteredState(ISantaService santaService, IAdminNotificationService notificationService, ITelegramBotClient client)
    {
        Commands = new(StringComparer.OrdinalIgnoreCase)
        {
            { "*", (HandleWishesResponse, ParticipantReadyState.StateName) },
            { "No", (HandleNoResponse, ParticipantReadyState.StateName) }
        };
        _santaService = santaService;
        _notificationService = notificationService;
        _client = client;
    }

    private async Task HandleWishesResponse(Message message, CancellationToken ct)
    {
        var participant = await _santaService.GetParticipant(message.Chat.Id, ct);
        await _santaService.UpdateParticipant(participant with { Wishes = message.Text });

        await SendNewParticipantNotification(message.Chat.Username, ct);

        const string text = "Thank you!\n"
            + "You're now registered in our presents exchange program. I will keep in touch with you on any updates.";

        await _client.SendTextMessageAsync(message.Chat.Id, text, replyMarkup: new ReplyKeyboardRemove(), cancellationToken: ct);
    }

    private async Task HandleNoResponse(Message message, CancellationToken ct)
    {
        await SendNewParticipantNotification(message.Chat.Username, ct);

        const string text = "Okay, then let your Secret Santa handle this themselves.\n"
            + "You're now registered in our presents exchange program. I will keep in touch with you on any following updates.";

        await _client.SendTextMessageAsync(message.Chat.Id, text, replyMarkup: new ReplyKeyboardRemove(), cancellationToken: ct);
    }

    private async Task SendNewParticipantNotification(string username, CancellationToken ct)
    {
        var notification = new NewParticipantNotification(username);
        await _notificationService.Notify(notification, ct);
    }
}

