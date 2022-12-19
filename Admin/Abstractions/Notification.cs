namespace SecretPepeBot.Admin.Abstractions;

public abstract class Notification
{
    public string Message { get; set; }

    public Notification(string message)
    {
        Message = message;
    }
}
