using SecretPepeBot;
using SecretPepeBot.Admin;
using SecretPepeBot.Admin.Abstractions;
using SecretPepeBot.Client;
using SecretPepeBot.Client.Abstractions;
using SecretPepeBot.Client.States;
using Telegram.Bot;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        var section = context.Configuration.GetSection(BotOptions.Section);
        var options = new BotOptions();
        section.Bind(options);
        services.Configure<BotOptions>(section);

        services.AddScoped<IAdminNotificationService, AdminNotificationService>();
        services.AddScoped<AdminUpdateHandler>();
        services.AddScoped<ClientUpdateHandler>();
        services.AddSingleton<ISantaService, SantaService>();
        services.AddSingleton<IClientState, NotStartedState>();
        services.AddSingleton<IClientState, StartMessageSentState>();
        services.AddSingleton<IClientState, ParticipantRegisteredState>();
        services.AddSingleton<IClientState, ParticipantReadyState>();
        services.AddSingleton<IClientStateFactory, ClientStateFactory>();

        services.AddBotClient<IAdminTelegramBotClient, AdminTelegramBotClient, AdminUpdateHandler>( 
            new AdminTelegramBotClient(options.AdminKey));

        services.AddBotClient<ITelegramBotClient, TelegramBotClient, ClientUpdateHandler>(
            new TelegramBotClient(options.ClientKey));
    })
    .Build();

while (true)
{
    try
    {
        await host.RunAsync();
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
}
