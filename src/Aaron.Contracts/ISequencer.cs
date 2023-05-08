namespace Aaron.Contracts;

public interface ISequencer
{
    public ulong Current { get; }
    /// <summary>
    /// generate next value
    /// </summary>
    /// <returns></returns>
    public ulong Next();
}