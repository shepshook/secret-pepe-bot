using SecretPepeBot.Client.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SecretPepeBot.Client.States;

public class StartMessageSentState : ClientStateBase
{
    public const string StateName = "started";
    public override string Name => StateName;

    private readonly ITelegramBotClient _client;
    private readonly ISantaService _santaService;

    protected override Dictionary<string, (HandleDelegate Handler, string NextState)> Commands { get; }

    public StartMessageSentState(ITelegramBotClient client, ISantaService santaService)
    {
        Commands = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Yes", (HandleYesResponse, ParticipantRegisteredState.StateName) },
            { "No", (HandleNoResponse, NotStartedState.StateName) }
        };
        _client = client;
        _santaService = santaService;
    }

    private async Task HandleYesResponse(Message message, CancellationToken ct)
    {
        const string startMessage = "Great!\n"
            + "It would also be nice to have some ideas on what kind of present you'd like to get. Do you want something special?\n"
            + "Share your ideas in the input below or press 'No' to skip this step.";

        var replies = new ReplyKeyboardMarkup(new KeyboardButton[] { "No" })
        {
            OneTimeKeyboard = true,
            ResizeKeyboard = true,
            InputFieldPlaceholder = "Share your thoughts"
        };

        await _client.SendTextMessageAsync(message.Chat.Id, startMessage, replyMarkup: replies, cancellationToken: ct);
    }

    private Task HandleNoResponse(Message message, CancellationToken ct)
    {
        const string text = "Sorry to hear that 🥲\nI'll be here for you if you change your mind.";

        return _client.SendTextMessageAsync(message.Chat.Id, text, replyMarkup: new ReplyKeyboardRemove(), cancellationToken: ct);
    }
}

