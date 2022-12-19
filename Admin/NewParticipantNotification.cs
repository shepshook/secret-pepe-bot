using SecretPepeBot.Admin.Abstractions;

namespace SecretPepeBot.Admin;

public class NewParticipantNotification : Notification
{
    public string ParticipantId { get; set; }

    public NewParticipantNotification(string participantId)
        : base($"Look who has joined us: @{participantId}")
    {
        ParticipantId = participantId;
    }
}
