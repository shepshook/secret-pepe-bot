using Telegram.Bot.Types;

namespace SecretPepeBot.Client.Abstractions;

public interface IClientState
{
    string Name { get; }

    bool CanHandle(string command);

    Task Handle(IStateMachine machine, string command, Message message, CancellationToken ct);
}

