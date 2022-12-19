namespace SecretPepeBot;

public interface ISantaService
{
    IReadOnlyCollection<Participant> GetParticipants(CancellationToken ct = default);

    Task AddParticipant(Participant participant, CancellationToken ct = default);

    Task UpdateParticipant(Participant participant, CancellationToken ct = default);

    Task<Participant?> GetParticipant(long chatId, CancellationToken ct = default);

    Task<IEnumerable<(Participant From, Participant To)>> GeneratePairs(CancellationToken ct = default);
}
