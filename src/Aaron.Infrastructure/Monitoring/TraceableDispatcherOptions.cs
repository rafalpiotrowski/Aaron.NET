
using System.Collections.Immutable;
using System.Reflection;
using Akka.Actor.Setup;
using Akka.Remote;

namespace Aaron.Infrastructure.Monitoring;

public class TraceableDispatcherOptions : Setup
{
    public ImmutableHashSet<Assembly> TraceableAssemblies { get; private set; } = ImmutableHashSet<Assembly>.Empty
        .Add(typeof(RemoteSettings).Assembly);

    public TraceableDispatcherOptions CleanTraceableAssemblies()
    {
        TraceableAssemblies = ImmutableHashSet<Assembly>.Empty;
        return this;
    }

    public TraceableDispatcherOptions AddTraceableAssembly(Assembly assembly)
    {
        TraceableAssemblies = TraceableAssemblies.Add(assembly);
        return this;
    }

    public TraceableDispatcherOptions AddTraceableAssemblyOf<TType>()
    {
        TraceableAssemblies = TraceableAssemblies.Add(typeof(TType).Assembly);
        return this;
    }
}
