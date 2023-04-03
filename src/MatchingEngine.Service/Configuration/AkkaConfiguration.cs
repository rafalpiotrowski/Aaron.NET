using Aaron.Actors;
using Akka.Actor;
using Akka.Cluster.Hosting;
using Akka.Cluster.Sharding;
using Akka.Hosting;
using Aaron.Configuration;
using MatchingEngine.Actors;
using MatchingEngine.Contracts;

namespace MatchingEngine.Service.Configuration;

public static class AkkaConfiguration
{
    public static AkkaConfigurationBuilder ConfigureMatchingEngineActors(this AkkaConfigurationBuilder builder,
        IServiceProvider serviceProvider)
    {
        var settings = serviceProvider.GetRequiredService<AkkaSettings>();
        var extractor = CreateMatchingEngineMessageRouter();

        if (settings.UseClustering)
        {
            return builder.WithShardRegion<MatchingEngineActor>("matchingengine",
                (system, registry, resolver) => s => Props.Create(() => new MatchingEngineActor(s)),
                extractor, settings.ShardOptions);
        }
        else
        {
            return builder.WithActors((system, registry, resolver) =>
            {
                var parent =
                    system.ActorOf(
                        GenericChildPerEntityParent.Props(extractor, s => Props.Create(() => new MatchingEngineActor(s))),
                        "matchingengines");
                registry.Register<MatchingEngineActor>(parent);
            });
        }
    }

    public static HashCodeMessageExtractor CreateMatchingEngineMessageRouter()
    {
        var extractor = HashCodeMessageExtractor.Create(30, o =>
        {
            return o switch
            {
                IWithEngineId msg => msg.EngineId,
                ShardRegion.StartEntity startEntity => startEntity.EntityId,
                _ => string.Empty
            };
        }, o => o);
        return extractor;
    }
}