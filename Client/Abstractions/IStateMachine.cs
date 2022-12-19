using Telegram.Bot.Types;

namespace SecretPepeBot.Client.Abstractions;

public interface IStateMachine
{
    Task TransformState(Message message, string stateName, CancellationToken ct);
}

