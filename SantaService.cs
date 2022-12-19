using System.Text.Json;
using Microsoft.Extensions.Options;

namespace SecretPepeBot;

public class SantaService : ISantaService
{
    public const string DataFile = "data.json";
    private readonly BotOptions _options;
    private readonly List<Participant> _participants;

    public SantaService(IOptions<BotOptions> options)
    {
        _options = options.Value;
        _participants = LoadParticipants() ?? new List<Participant>();
    }

    public IReadOnlyCollection<Participant> GetParticipants(CancellationToken ct = default) => _participants.Where(x => x.State == "ready").ToList();

    public async Task AddParticipant(Participant participant, CancellationToken ct = default)
    {
        _participants.Add(participant);
        await SaveParticipants();
    }

    public Task<IEnumerable<(Participant From, Participant To)>> GeneratePairs(CancellationToken ct = default)
    {
        var givers = new HashSet<Participant>(_participants);
        var receivers = new HashSet<Participant>(_participants);

        var result = new HashSet<(Participant From, Participant To)>();

        try
        {
            foreach (var giver in givers)
            {
                // exclude giver themselves
                var excludedReceivers = new HashSet<Participant> { giver };

                // exclude receivers from exception pairs
                var exceptionPairs = _options.GetExceptionPairs();
                if (exceptionPairs.Select(x => x.From).Contains(giver.UserId))
                {
                    var exceptionReceivers = exceptionPairs
                        .Where(x => x.From == giver.UserId)
                        .Select(x => receivers.SingleOrDefault(y => y.UserId == x.To))
                        .Where(x => x != null);

                    exceptionReceivers.ToList().ForEach(x => excludedReceivers.Add(x));
                }

                // exclude giver <-> receiver mutual exchange
                if (result.Select(x => x.To).Contains(giver))
                {
                    excludedReceivers.Add(result.Single(x => x.To == giver).From);
                }

                var filteredReceivers = new HashSet<Participant>(receivers);
                filteredReceivers.ExceptWith(excludedReceivers);

                var receiver = filteredReceivers.OrderBy(_ => Random.Shared.Next()).First();
                receivers.Remove(receiver);

                result.Add((giver, receiver));
            }
        }
        catch (Exception)
        {
            return GeneratePairs();
        }

        return Task.FromResult<IEnumerable<(Participant From, Participant To)>>(result);
    }

    public Task<Participant?> GetParticipant(long chatId, CancellationToken ct = default)
    {
        return Task.FromResult<Participant?>(_participants.SingleOrDefault(x => x.ChatId == chatId));
    }

    public async Task<bool> RemoveParticipant(string userId, CancellationToken ct = default)
    {
        var participant = _participants.SingleOrDefault(x => x.UserId == userId);

        if (participant != null)
        {
            _participants.Remove(participant);
            await SaveParticipants(ct);
            return true;
        }

        return false;
    }

    public Task UpdateParticipant(Participant participant, CancellationToken ct = default)
    {
        var oldParticipant = _participants.Single(x => x.ChatId == participant.ChatId);

        _participants.Remove(oldParticipant);
        _participants.Add(participant);

        return SaveParticipants(ct);
    }

    private List<Participant> LoadParticipants()
    {
        if (!File.Exists(DataFile))
        {
            return new List<Participant>();
        }

        var json = File.ReadAllText(DataFile);
        return JsonSerializer.Deserialize<List<Participant>>(json);
    }

    private Task SaveParticipants(CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(_participants);
        return File.WriteAllTextAsync(DataFile, json, ct);
    }
}