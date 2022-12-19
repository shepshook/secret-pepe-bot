using SecretPepeBot.Admin.Abstractions;
using Telegram.Bot;

namespace SecretPepeBot.Admin;

public class AdminTelegramBotClient : TelegramBotClient, IAdminTelegramBotClient
{
    public AdminTelegramBotClient(TelegramBotClientOptions options, HttpClient? httpClient = null) : base(options, httpClient)
    { }

    public AdminTelegramBotClient(string token, HttpClient? httpClient = null) : base(token, httpClient)
    { }
}
