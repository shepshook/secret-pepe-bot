using SecretPepeBot.Client.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace SecretPepeBot.Client.States;

public class NotStartedState : ClientStateBase
{
    public const string StateName = "not-started";
    public override string Name => StateName;

    private readonly ITelegramBotClient _client;
    private readonly ISantaService _santaService;

    protected override Dictionary<string, (HandleDelegate Handler, string NextState)> Commands { get; }

    public NotStartedState(ITelegramBotClient client, ISantaService santaService)
    {
        _client = client;
        _santaService = santaService;
        Commands = new(StringComparer.OrdinalIgnoreCase)
        {
            { "/start", (SendStartMessage, StartMessageSentState.StateName) }
        };
    }

    public async Task SendStartMessage(Message message, CancellationToken ct)
    {
        var participant = new Participant(message.Chat.Username, message.Chat.Id, default, Name);
        await _santaService.AddParticipant(participant);

        const string startMessage = "Greetings from the Secret Pepe 🎅\n"
            + "I'm really happy to have you invited in our presents exchange program. Will you agree to take part?";

        var replies = new ReplyKeyboardMarkup(new KeyboardButton[] { "Yes", "No" })
        {
            OneTimeKeyboard = true,
            ResizeKeyboard = true
        };

        await _client.SendTextMessageAsync(message.Chat.Id, startMessage, replyMarkup: replies, cancellationToken: ct);
    }
}

