using Aaron.Contracts;
using Aaron.Contracts.Commands;
using Aaron.Contracts.Events;
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

        Add<Start>("start");
        Add<Cancel>("cancel");
        Add<Finished>("finished");
        Add<Failed>("failed");
        Add<Cancelled>("cancelled");
    }

    public MessageSerializer(ExtendedActorSystem system) : base(1001, system) { }
}
