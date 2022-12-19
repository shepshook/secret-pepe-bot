using SecretPepeBot.Client.Abstractions;

namespace SecretPepeBot.Client.States;

public class ParticipantReadyState : ClientStateBase
{
    public const string StateName = "ready";
    public override string Name => StateName;

    protected override Dictionary<string, (HandleDelegate Handler, string NextState)> Commands => new();
}

