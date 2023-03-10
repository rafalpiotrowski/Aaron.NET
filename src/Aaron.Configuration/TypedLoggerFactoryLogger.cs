
using System.Text;
using Akka.Actor;
using Akka.Configuration;
using Akka.Dispatch;
using Akka.Event;
using Microsoft.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;
using Event = Akka.Event;
using Akka.Hosting.Logging;

namespace Aaron.Configuration;

/// <summary>
/// Copy of <see cref="Akka.Hosting.Logging.LoggerFactoryLogger"/>.
/// Better handles logger category, expands args into state
/// </summary>
public class TypedLoggerFactoryLogger : ActorBase, IRequiresMessageQueue<ILoggerMessageQueueSemantics>
{
    public const string DefaultTimeStampFormat = "yy/MM/dd-HH:mm:ss.ffff";
    private const string DefaultMessageFormat = "[{{Timestamp:{0}}}][{{LogSource}}][{{ActorPath}}][{{Thread:0000}}]: {{Message}}";
    private static readonly Event.LogLevel[] AllLogLevels = Enum.GetValues(typeof(Event.LogLevel)).Cast<Event.LogLevel>().ToArray();

    /// <summary>
    /// only used when we're shutting down / spinning up
    /// </summary>
    private readonly ILoggingAdapter _internalLogger = Event.Logging.GetLogger(Context.System.EventStream, nameof(TypedLoggerFactoryLogger));
    private readonly ILoggerFactory _loggerFactory;
    private readonly string _messageFormat;

    public TypedLoggerFactoryLogger()
    {
        _messageFormat = string.Format(DefaultMessageFormat, DefaultTimeStampFormat);
        var setup = Context.System.Settings.Setup.Get<LoggerFactorySetup>();
        if (!setup.HasValue)
            throw new ConfigurationException(
                $"Could not start {nameof(TypedLoggerFactoryLogger)}, the required setup class " +
                $"{nameof(LoggerFactorySetup)} could not be found. Have you added this to the ActorSystem setup?");
        _loggerFactory = setup.Value.LoggerFactory;
    }

    protected override bool Receive(object message)
    {
        switch (message)
        {
            case InitializeLogger _:
                _internalLogger.Info($"{nameof(TypedLoggerFactoryLogger)} started");
                Sender.Tell(new LoggerInitialized());
                return true;

            case LogEvent logEvent:
                Log(logEvent, Sender.Path);
                return true;

            default:
                return false;
        }
    }

    private void Log(LogEvent log, ActorPath path)
    {
        var logger = _loggerFactory.CreateLogger(log.LogClass);
        if (log.Message is LogMessage m)
        {
            var args = m.Parameters().ToArray();
            Array.Resize(ref args, args.Length + 1);
            args[^1] = path.ToString();
            logger.Log(GetLogLevel(log.LogLevel()), log.Cause, m.Format, args);
        }
        else
        {
            var message = GetMessage(log.Message);
            logger.Log(GetLogLevel(log.LogLevel()), log.Cause, _messageFormat, log.Timestamp, log.LogSource, path.ToString(), log.Thread.ManagedThreadId, message);
        }
    }

    private static object GetMessage(object obj)
    {
        try
        {
            return obj switch
            {
                LogMessage m => string.Format(m.Format, m.Parameters()),
                string m => m,
                null => string.Empty,
                _ => obj.ToString()
            } ?? string.Empty;
        }
        catch (Exception ex)
        {
            // Formatting/ToString error handling
            var sb = new StringBuilder("Exception while recording log: ")
                .Append(ex.Message)
                .Append(' ');
            switch (obj)
            {
                case LogMessage msg:
                    var args = msg.Parameters().Select(o =>
                    {
                        try
                        {
                            return o.ToString();
                        }
                        catch (Exception e)
                        {
                            return $"{o.GetType()}.ToString() throws {e.GetType()}: {e.Message}";
                        }
                    });
                    sb.Append($"Format: [{msg.Format}], Args: [{string.Join(",", args)}].");
                    break;
                case string str:
                    sb.Append($"Message: [{str}].");
                    break;
                default:
                    sb.Append($"Failed to invoke {obj?.GetType()}.ToString().");
                    break;
            }

            sb.AppendLine(" Please take a look at the logging call where this occurred and fix your format string.");
            sb.Append(ex);
            return sb.ToString();
        }
    }

    private static LogLevel GetLogLevel(Event.LogLevel level)
    => level switch
        {
            Event.LogLevel.DebugLevel => LogLevel.Debug,
            Event.LogLevel.InfoLevel => LogLevel.Information,
            Event.LogLevel.WarningLevel => LogLevel.Warning,
            Event.LogLevel.ErrorLevel => LogLevel.Error,
            _ => LogLevel.Error
        };
}