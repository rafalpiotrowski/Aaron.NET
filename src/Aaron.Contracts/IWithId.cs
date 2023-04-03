namespace Aaron.Contracts;

/// <summary>
/// Marking interface for messages that have an Id of type T
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IWithId<out T>
{
    T Id { get;  }
}