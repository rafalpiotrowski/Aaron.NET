using Akka.Persistence;

namespace MatchingEngine.Actors;

public class MatchingEngineActor : ReceivePersistentActor
{
    /// <summary>
    /// should never be changed
    /// </summary>
    public const string MatchingEngineActorPersistenceIdPrefix = "me";
    /// <summary>
    /// should never be changed
    /// </summary>
    public const string MatchingEngineActorPersistenceIdSeperator = "_";
    public override string PersistenceId { get; }

    public MatchingEngineActor(string entityName)
    {
        PersistenceId = $"{MatchingEngineActorPersistenceIdPrefix}{MatchingEngineActorPersistenceIdSeperator}{entityName}";
    }

}