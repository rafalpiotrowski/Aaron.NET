using Aaron.Configuration;
using Microsoft.Extensions.Logging;

namespace Akka.Hosting;

public static class LoggingExtensions
{
    /// <summary>
    /// Add the <see cref="ILoggerFactory"/> logger that sinks all log events to the default
    /// <see cref="ILoggerFactory"/> instance registered in the host <see cref="ServiceProvider"/>
    /// </summary>
    /// <param name="configBuilder">The <see cref="LoggerConfigBuilder"/> instance </param>
    /// <returns>the original <see cref="LoggerConfigBuilder"/> used to configure the logger system</returns>
    public static LoggerConfigBuilder AddTypedLoggerFactory(this LoggerConfigBuilder configBuilder)
    {
        configBuilder.AddLogger<TypedLoggerFactoryLogger>();
        return configBuilder;
    }
}