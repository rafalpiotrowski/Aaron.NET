using Aaron.Contracts;

namespace Symbology.Actors;

public sealed class InstrumentActor : Aaron.Actors.PersistentActor
{
    public const string PersistenceIdPrefix = "i";
    
    protected override PersistenceId Id { get; }
    
    public InstrumentActor(string entityId)
    {
        Id = new()
        {
            Prefix = PersistenceIdPrefix,
            Delimiter = Aaron.Contracts.PersistenceId.UnderscoreDelimiter,
            EntityId = entityId
        };
    }

    
}