using SecretPepeBot.Client.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SecretPepeBot.Client;

public class ClientUpdateHandler : ITelegramUpdateHandler, IStateMachine
{
    private readonly ITelegramBotClient _client;
    private readonly ISantaService _santaService;
    private readonly IClientStateFactory _stateFactory;

    public ClientUpdateHandler(
        ITelegramBotClient client,
        ISantaService santaService,
        IClientStateFactory stateFactory)
    {
        _client = client;
        _santaService = santaService;
        _stateFactory = stateFactory;
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
        if (message.Text is not { } messageText)
        {
            return;
        }

        var participant = await _santaService.GetParticipant(message.Chat.Id, ct);
        var state = participant != null
            ? _stateFactory.GetState(participant.State)
            : _stateFactory.GetInitialState();

        if (state.CanHandle(messageText))
        {
            await state.Handle(this, messageText, message, ct);
        }
        else
        {
            await SendUnknownMessageResponse(message, ct);
        }
    }

    private Task<Message> SendUnknownMessageResponse(Message message, CancellationToken ct)
    {
        const string sorryResponse = "Sorry, I didn't get what you were trying to say.";

        return _client.SendTextMessageAsync(message.Chat.Id, sorryResponse, cancellationToken: ct);
    }

    public async Task TransformState(Message message, string stateName, CancellationToken ct)
    {
        var participant = await _santaService.GetParticipant(message.Chat.Id, ct);
        await _santaService.UpdateParticipant(participant with { State = stateName }, ct);
    }
}
