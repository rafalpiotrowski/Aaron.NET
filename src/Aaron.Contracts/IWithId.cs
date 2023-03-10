namespace Aaron.Contracts;

public interface IWithId<out T>
{
    T Id { get;  }
}