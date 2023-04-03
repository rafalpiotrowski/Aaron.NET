using Aaron.Contracts;
using Akka.Actor;

namespace Aaron.Infrastructure.Serialization;

public sealed class MessageSerializer : ProtobufMessageSerializer<MessageSerializer>
{
    static MessageSerializer()
    {
        Add<Currency>("ccy");
        Add<Money>("money");
        Add<ExchangeRate>("exrate");
        Add<Country>("country");
    }

    public MessageSerializer(ExtendedActorSystem system) : base(1002, system) { }
}
