using System.Collections.Immutable;

namespace Aaron.Infrastructure;

public sealed class TelemetryConstants
{
    public static readonly ImmutableHashSet<string> DurationUnits = ImmutableHashSet<string>.Empty.Add("ms").Add("s").Add("m").Add("h");

    public const string TelemetryNamespace = "aaron";
    
    public const string SystemTagName = "system";
    public const string ActorTagName = "actor";
    public const string MessageTagName = "message";
    public const string HandledTagName = "handled";
    public const string UnhandledTagName = "unhandled";
    public const string UndefinedActorName = "undefined";
}