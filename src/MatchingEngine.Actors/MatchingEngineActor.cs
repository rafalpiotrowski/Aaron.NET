using Aaron.Actors;
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
    /// <summary>
    /// should never be changed
    /// </summary>
    public const string MatchingEngineActorPersistenceIdSeperator = "_";
    public sealed override string PersistenceId { get; }

    /// <summary>
    /// TBD
    /// </summary>
    /// <param name="entityName">instrument code</param>
    public MatchingEngineActor(string entityName)
    {
        PersistenceId = $"{MatchingEngineActorPersistenceIdPrefix}{MatchingEngineActorPersistenceIdSeperator}{entityName}";

        Logger.Info($"Matching Engine: {PersistenceId} created");
        
        Command<MatchOrder>(cmd =>
        {
            Logger.Info($"Handling: {cmd}");
            Sender.Tell(new OrderMatched() { OrderId = cmd.OrderId, Price = decimal.Zero, Units = 0}, Self);
        });
    }
    
    
}