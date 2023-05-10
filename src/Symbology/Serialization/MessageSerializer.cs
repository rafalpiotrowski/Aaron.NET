using Aaron.Infrastructure.Serialization;
using Akka.Actor;
using Symbology.Contracts;
using Symbology.Contracts.Commands;
using Symbology.Contracts.Events;

namespace Symbology.Serialization;

public sealed class MessageSerializer : ProtobufMessageSerializer<MessageSerializer>
{
    static MessageSerializer()
    {
        Add<Instrument>("i");

        Add<AddInstrument>("ai");
        Add<AddInstrumentOk>("ai_ok");
        Add<AddInstrumentFailed>("ai_err");
    }

    public MessageSerializer(ExtendedActorSystem system) : base(1002, system) { }
}