using Adaptive.Agrona.Concurrent;
using Adaptive.Cluster.Service;
using Akka.Actor;

namespace Gateway.Service;

public class AeronClusterBackgroundService : IHostedService
{
    private readonly ILogger<AeronClusterBackgroundService> _logger;
    private readonly ILogger<AeronGatewayService> _aeronLogger;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly ActorSystem _actorSystem;
    private readonly AtomicBoolean _isRunning = new AtomicBoolean(false);

    private ClusteredServiceContainer.Context _context;

    public AeronClusterBackgroundService(
        ILogger<AeronClusterBackgroundService> logger,
        ILogger<AeronGatewayService> aeronLogger,
        IHostApplicationLifetime appLifetime)
    {
        _logger = logger;
        _aeronLogger = aeronLogger;
        _appLifetime = appLifetime;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting");
        
        _appLifetime.ApplicationStopping.Register(OnStopping);
        _appLifetime.ApplicationStopped.Register(OnStopped);
        
        _context = new ClusteredServiceContainer.Context()
            .ClusteredService(new AeronGatewayService(_aeronLogger, _actorSystem));

        var containerThread = new ThreadStart(() =>
        {
            using (ClusteredServiceContainer.Launch(_context))
            {
                _logger.LogInformation("Started Service Container...");

                _context.ShutdownSignalBarrier().Await();

                _logger.LogInformation("Stopping Service Container...");
            }

            _logger.LogInformation("Stopped.");

        });

        var t = new Thread(containerThread);
        t.Start();

        _isRunning.Set(true);

        try
        {
            while (!cancellationToken.IsCancellationRequested && _isRunning.Get())
            {
                _logger.LogInformation("running");
                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }
        finally
        {
            // _logger.LogInformation("Signaling container to stop");
            // _context.ShutdownSignalBarrier().Signal();
            _logger.LogInformation("Stopped");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping...");
        _isRunning.Set(false);
        return Task.CompletedTask;
    }
    
    private void OnStopping()
    {
        _logger.LogInformation("Application is stopping...");
        if(_isRunning.Get())
        _isRunning.Set(false);
        _logger.LogInformation("Signaling container to stop");
        _context.ShutdownSignalBarrier().Signal();
        // Other cleanup code here...
    }

    private void OnStopped()
    {
        _logger.LogInformation("Application has stopped.");
        // Other cleanup code here...
    }
}