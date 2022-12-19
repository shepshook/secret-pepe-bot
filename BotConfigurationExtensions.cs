using SecretPepeBot.Client.Abstractions;
using Telegram.Bot;

public static class BotConfigurationExtensions
{
    public static IServiceCollection AddBotClient<TService, TImpl, TUpdateHandler>(this IServiceCollection services, TImpl implementation)
        where TService : class, ITelegramBotClient
        where TImpl : TelegramBotClient, TService
        where TUpdateHandler : ITelegramUpdateHandler
    {
        services.AddScoped<TService>(_ => implementation);
        implementation.StartReceiving(
            (_, update, ct) =>
            {
                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<TUpdateHandler>();

                return handler.HandleUpdateAsync(update, ct);
            }, 
            (client, exception, ct) => 
            {
                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var logger = scope.ServiceProvider.GetRequiredService<ILogger>();
                
                logger.LogError(exception, "Something bad has happened.");

                return Task.CompletedTask;
            });

        return services;
    }
}
