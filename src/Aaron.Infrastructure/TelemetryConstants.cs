using System.Collections.Immutable;

namespace Aaron.Infrastructure;

public sealed class TelemetryConstants
{
    public static readonly ImmutableHashSet<string> DurationUnits = ImmutableHashSet<string>.Empty.Add("ms").Add("s").Add("m").Add("h");

    public const string TelemetryNamespace = "aaron";
}