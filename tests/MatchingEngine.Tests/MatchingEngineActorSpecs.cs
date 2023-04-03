using Aaron.Configuration;
using Aaron.Contracts;
using Akka.Hosting;
using Akka.Hosting.TestKit;
using FluentAssertions;
using MatchingEngine.Actors;
using MatchingEngine.Contracts;
using MatchingEngine.Service.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit.Abstractions;

namespace MatchingEngine.Tests;

public class MatchingEngineActorSpecs : TestKit
{
    public MatchingEngineActorSpecs(ITestOutputHelper output) : base(output: output)
    {
    }

    [Fact]
    public void MatchingEngineActor_should_follow_Protocol()
    {
        // arrange (engine actor parent is already running)
        var engineActor = ActorRegistry.Get<MatchingEngineActor>();
        var engineId = "APPLE";
        var messages = new IWithEngineId[]
        {
            new MatchOrder {EngineId = engineId, OrderId = 1, Side = Side.Buy, Units = 100, Price = 100.10M },
            //new MatchOrder {EngineId = engineId, OrderId = 2, Side = Side.Sell, Units = 100, Price = 100.05M }
        };

        // act

        foreach (var msg in messages)
        {
            engineActor.Tell(msg, TestActor);
        }

        // assert
        var match = (OrderMatched)FishForMessage(c => c is OrderMatched);
        match.OrderId.Should().Be(1);
        Assert.StrictEqual(decimal.Zero,match.Units);
    }

    protected override void ConfigureServices(HostBuilderContext context, IServiceCollection services)
    {
        var settings = new AkkaSettings() { UseClustering = false, PersistenceMode = PersistenceMode.InMemory };
        services.AddSingleton(settings);
        base.ConfigureServices(context, services);
    }

    protected override void ConfigureAkka(AkkaConfigurationBuilder builder, IServiceProvider provider)
    {
        builder.ConfigureMatchingEngineActors(provider).ConfigurePersistence(provider);
    }
}