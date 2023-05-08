using Aaron.Actors;
using Akka.Actor;
using Akka.Cluster.Hosting;
using Akka.Cluster.Sharding;
using Akka.Hosting;
using Aaron.Configuration;
using Aaron.Contracts;
using Symbology.Actors;
using Symbology.Contracts;

namespace Symbology.Service.Configuration;

public static class AkkaConfiguration
{
    public static AkkaConfigurationBuilder ConfigureSymbologyActors(this AkkaConfigurationBuilder builder,
        IServiceProvider serviceProvider)
    {
        var settings = serviceProvider.GetRequiredService<AkkaSettings>();
        var extractor = CreateMatchingEngineMessageRouter();

        if (settings.UseClustering)
        {
            return builder.WithShardRegion<InstrumentActor>("instrument",
                (system, registry, resolver) => s => Props.Create(() => new InstrumentActor(s)),
                extractor, settings.ShardOptions);
        }
        else
        {
            return builder.WithActors((system, registry, resolver) =>
            {
                var parent =
                    system.ActorOf(
                        GenericChildPerEntityParent.Props(extractor, s => Props.Create(() => new InstrumentActor(s))),
                        "instruments");
                registry.Register<InstrumentActor>(parent);
            });
        }
    }

    public static HashCodeMessageExtractor CreateMatchingEngineMessageRouter()
    {
        var extractor = HashCodeMessageExtractor.Create(30, o =>
        {
            return o switch
            {
                IWithId<InstrumentId> msg => msg.Id.ToString(),
                ShardRegion.StartEntity startEntity => startEntity.EntityId,
                _ => string.Empty
            };
        }, o => o);
        return extractor;
    }
}