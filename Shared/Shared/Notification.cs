namespace Shared;


public class Notification
{
    public List<string>? Messages { get; set; }
    public bool Success { get; set; }

    public Notification()
    {
        Messages = new List<string>();
    }
}

public class Notification<T> : Notification
{
    public T Value { get; set; }
}
