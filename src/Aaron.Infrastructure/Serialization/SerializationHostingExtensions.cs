using Aaron.Contracts;
using Akka.Hosting;

namespace Aaron.Infrastructure.Serialization;


public static class SerializationHostingExtensions
{
    /// <summary>
    /// Configures the custom serialization for Aaron Contracts messages.
    /// </summary>
    public static AkkaConfigurationBuilder AddAaronContractsSerialization(this AkkaConfigurationBuilder builder)
    {
        return builder.WithCustomSerializer("aaron", new[] { typeof(IProtocolMember) },
            system => new MessageSerializer(system));
    }
}
