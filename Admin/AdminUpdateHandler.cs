using System.Text;
using Microsoft.Extensions.Options;
using SecretPepeBot.Admin.Abstractions;
using SecretPepeBot.Client.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SecretPepeBot.Admin;

public class AdminUpdateHandler : ITelegramUpdateHandler
{
    private readonly IAdminTelegramBotClient _adminClient;
    private readonly ITelegramBotClient _simpleClient;
    private readonly ISantaService _santaService;
    private readonly BotOptions _options;
    private readonly ILogger<AdminUpdateHandler> _logger;

    public AdminUpdateHandler(IAdminTelegramBotClient adminClient, ISantaService santaService, ITelegramBotClient simpleClient, IOptions<BotOptions> options, ILogger<AdminUpdateHandler> logger)
    {
        _adminClient = adminClient;
        _santaService = santaService;
        _simpleClient = simpleClient;
        _options = options.Value;
        _logger = logger;
    }

    public Task HandleUpdateAsync(Update update, CancellationToken ct)
    {
        return update switch
        {
            { Message: { } message } => BotOnMessageReceived(message, ct),
            _ => Task.CompletedTask
        };
    }

    private async Task BotOnMessageReceived(Message message, CancellationToken ct)
    {
        if (message.Chat.Id != _options.AdminChatId)
        {
            await SendUnknownMessageResponse(message, ct);
            return;
        }

        if (message.Text is not { } messageText)
        {
            await SendUnknownMessageResponse(message, ct);
            return;
        }
        
        var sentMessage = await (messageText.Split(' ')[0] switch
        {
            "/start" => SendStartMessage(message, ct),
            "/list" => SendListResponse(message, ct),
            "/generate" => SendGenerateResponse(message, ct),
            _ => SendUnknownMessageResponse(message, ct)
        });
    }

    private Task<Message> SendStartMessage(Message message, CancellationToken ct)
    {
        var startMessage = "Hello! This is the admin bot for your Secret Pepe service.\n"
            + "Usage:\n"
            + "/list - get current participants list;\n"
            + "/generate - send message with a recipient info to every participant.\n"
            + "This bot will also notify you on changes in participants list.\n"
            + $"By the way, your chat ID is {message.Chat.Id}, this might be handy 😉";

        return _adminClient.SendTextMessageAsync(message.Chat.Id, startMessage, cancellationToken: ct);
    }

    private Task<Message> SendListResponse(Message message, CancellationToken ct)
    {
        var participants = _santaService.GetParticipants();

        var listMessage = new StringBuilder($"You've got {participants.Count} participants registered:\n");
        foreach (var participant in participants)
        {
            listMessage.AppendLine(participant.UserId);
        }

        return _adminClient.SendTextMessageAsync(message.Chat.Id, listMessage.ToString(), cancellationToken: ct);
    }

    private async Task<Message> SendGenerateResponse(Message message, CancellationToken ct)
    {
        var pairs = await _santaService.GeneratePairs();

        foreach (var pair in pairs)
        {
            var matchMessage = $"Hello again!\n"
                + $"You're lucky to have @{pair.To.UserId} as your secret presentee.\n"
                + (string.IsNullOrEmpty(pair.To.Wishes)
                    ? "They didn't bother to wish anything special."
                    : $"Here is their wishes:\n\"{pair.To.Wishes}\"");

            await _simpleClient.SendTextMessageAsync(pair.From.ChatId, matchMessage, cancellationToken: ct);
        }

        const string successMessage = "Pairs generated successfully!";
        return await _adminClient.SendTextMessageAsync(message.Chat.Id, successMessage, cancellationToken: ct);
    }

    private Task<Message> SendUnknownMessageResponse(Message message, CancellationToken ct)
    {
        const string sorryResponse = "Sorry, I didn't get what you were trying to say.";

        return _adminClient.SendTextMessageAsync(message.Chat.Id, sorryResponse, cancellationToken: ct);
    }
}
