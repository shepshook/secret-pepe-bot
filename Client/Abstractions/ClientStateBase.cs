using Telegram.Bot.Types;

namespace SecretPepeBot.Client.Abstractions;

public abstract class ClientStateBase : IClientState
{
    protected const string WildcardCommand = "*";
    
    protected delegate Task HandleDelegate(Message message, CancellationToken ct);

    protected abstract Dictionary<string, (HandleDelegate Handler, string NextState)> Commands { get; }

    public abstract string Name { get; }

    public bool CanHandle(string command) => Commands.ContainsKey(command) || Commands.ContainsKey(WildcardCommand);

    public async Task Handle(IStateMachine machine, string command, Message message, CancellationToken ct)
    {
        if (!CanHandle(command))
        {
            return;
        }

        HandleDelegate handle;
        string nextState;

        if (Commands.TryGetValue(command, out var value))
        {
            (handle, nextState) = value;
        }
        else if (Commands.ContainsKey(WildcardCommand))
        {
            (handle, nextState) = Commands[WildcardCommand];
        }
        else
        {
            throw new InvalidOperationException();
        }

        await handle(message, ct);
        await machine.TransformState(message, nextState, ct);
    }
}

