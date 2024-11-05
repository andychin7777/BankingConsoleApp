namespace Shared;

public class Notification<T>
{
    public List<string>? Messages { get; set; }
    public T Value { get; set; }
}
