using Telegram.Bot.Types;

namespace SecretPepeBot;

public class BotOptions
{
    public const string Section = "Bot";

    public string ClientKey { get; set; }

    public string AdminKey { get; set; }

    public long AdminChatId { get; set; }

    public IEnumerable<IEnumerable<string>> ExceptionPairs { get; set; }

    public HashSet<(string From, string To)> GetExceptionPairs() =>
        new(ExceptionPairs?.SelectMany(pair => new[] 
        {
            (pair.First(), pair.Last()), 
            (pair.Last(), pair.First())
        }) 
        ?? Array.Empty<(string, string)>());
}