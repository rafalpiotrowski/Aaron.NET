using Akka.Hosting;
using Symbology.Contracts;

namespace Symbology.Serialization;


public static class SerializationHostingExtensions
{
    /// <summary>
    /// Configures the custom serialization for Aaron Contracts messages.
    /// </summary>
    public static AkkaConfigurationBuilder AddSymbologySerialization(this AkkaConfigurationBuilder builder)
    {
        return builder.WithCustomSerializer("symbology", new[] { typeof(IProtocolMember) },
            system => new MessageSerializer(system));
    }
}
