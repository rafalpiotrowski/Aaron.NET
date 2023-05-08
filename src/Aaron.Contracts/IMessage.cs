namespace Aaron.Contracts;

public interface IMessage : IProtocolMember
{
    public MessageHeader Header { get; }
}