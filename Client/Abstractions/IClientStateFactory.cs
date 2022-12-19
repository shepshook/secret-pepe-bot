namespace SecretPepeBot.Client.Abstractions;

public interface IClientStateFactory
{
    IClientState GetInitialState();

    IClientState GetState(string name);
}
