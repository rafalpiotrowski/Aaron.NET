using Aaron.Actors;
using Aaron.Contracts;
using Akka.Event;
using MatchingEngine.Contracts;

namespace MatchingEngine.Actors;

/// <summary>
/// Actor that will run MatchingEngine.Engine for given instrument
/// </summary>
public class MatchingEngineActor : PersistentActor
{
    /// <summary>
    /// should never be changed
    /// </summary>
    public const string MatchingEngineActorPersistenceIdPrefix = "me";

    protected override PersistenceId Id { get; }

    /// <summary>
    /// TBD
    /// </summary>
    /// <param name="entityName">instrument code</param>
    public MatchingEngineActor(string entityName)
    {
        Id = new()
        {
            Prefix = MatchingEngineActorPersistenceIdPrefix,
            Delimiter = Aaron.Contracts.PersistenceId.UnderscoreDelimiter,
            EntityId = entityName
        };

        Logger.Info($"Matching Engine: {PersistenceId} created");
        
        Command<MatchOrder>(cmd =>
        {
            Logger.Info($"Handling: {cmd}");
            Sender.Tell(new OrderMatched() { OrderId = cmd.OrderId, Price = decimal.Zero, Units = 0}, Self);
        });
    }
    
    
}