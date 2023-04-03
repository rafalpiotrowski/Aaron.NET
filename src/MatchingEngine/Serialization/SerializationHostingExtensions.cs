using Akka.Hosting;
using MatchingEngine.Contracts;

namespace MatchingEngine.Serialization;


public static class SerializationHostingExtensions
{
    /// <summary>
    /// Configures the custom serialization for Aaron Contracts messages.
    /// </summary>
    public static AkkaConfigurationBuilder AddMatchingEngineSerialization(this AkkaConfigurationBuilder builder)
    {
        return builder.WithCustomSerializer("matchingengine", new[] { typeof(IProtocolMember) },
            system => new MessageSerializer(system));
    }
}
