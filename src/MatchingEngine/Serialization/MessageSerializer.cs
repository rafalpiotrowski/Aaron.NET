using Aaron.Infrastructure.Serialization;
using Akka.Actor;
using MatchingEngine.Contracts;

namespace MatchingEngine.Serialization;

public sealed class MessageSerializer : ProtobufMessageSerializer<MessageSerializer>
{
    static MessageSerializer()
    {
        Add<MatchOrder>("matchorder");
        Add<OrderMatched>("ordermatched");
    }

    public MessageSerializer(ExtendedActorSystem system) : base(1003, system) { }
}