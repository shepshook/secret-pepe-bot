namespace SecretPepeBot.Admin.Abstractions;

public interface IAdminNotificationService
{
    Task Notify(Notification notification, CancellationToken ct);
}
