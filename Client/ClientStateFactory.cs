using SecretPepeBot.Client.Abstractions;

namespace SecretPepeBot.Client;

public class ClientStateFactory : IClientStateFactory
{
    private const string InitialState = "not-started";
    private readonly IEnumerable<IClientState> _states;

    public ClientStateFactory(IEnumerable<IClientState> states) => _states = states;

    public IClientState GetState(string name) => _states.Single(x => x.Name == name);

    public IClientState GetInitialState() => GetState(InitialState);
}
