using Aaron.Contracts;
using Akka.Actor;

namespace Aaron.Infrastructure.Serialization;

public sealed class MessageSerializer : ProtobufMessageSerializer<MessageSerializer>
{
    static MessageSerializer()
    {
        Add<Currency>("ccy");
        Add<Money>("m");
        Add<ExchangeRate>("exr");
        Add<Country>("co");
    }

    public MessageSerializer(ExtendedActorSystem system) : base(1001, system) { }
}
