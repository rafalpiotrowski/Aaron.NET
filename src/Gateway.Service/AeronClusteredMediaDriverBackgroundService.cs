using System.Diagnostics;
using Akka.Dispatch.SysMsg;
using Akka.Util;

namespace Gateway.Service;

public class AeronClusteredMediaDriverBackgroundService : IHostedService
{
    private readonly ILogger<AeronClusteredMediaDriverBackgroundService> _logger;
    private readonly IHostApplicationLifetime _appLifetime;
    private readonly AtomicBoolean _isRunning = new AtomicBoolean(false); 
    private Process? _process;

    public AeronClusteredMediaDriverBackgroundService(
        ILogger<AeronClusteredMediaDriverBackgroundService> logger,
        IHostApplicationLifetime appLifetime)
    {
        _logger = logger;
        _appLifetime = appLifetime;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting");
        
        _appLifetime.ApplicationStopping.Register(OnStopping);
        _appLifetime.ApplicationStopped.Register(OnStopped);
        
        var info = new ProcessStartInfo()
        {
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
            FileName = "c:/java/azulu11/bin/java",
            Arguments = "-cp c:/code/Aeron.NET/driver/media-driver.jar -Daeron.cluster.ingress.channel=aeron:udp?endpoint=localhost:9010 -Daeron.archive.control.channel=aeron:udp?endpoint=localhost:8010 -Daeron.archive.replication.channel=aeron:udp?endpoint=localhost:0 -Daeron.cluster.replication.channel=aeron:udp?endpoint=localhost:9011 -Daeron.cluster.members=\"0,localhost:20000,localhost:20001,localhost:20002,localhost:0,localhost:8010\" io.aeron.cluster.ClusteredMediaDriver",
        };
        _process = new Process()
        {
            StartInfo = info
        };
        
        _process.Exited += (sender, args) => _logger.LogWarning("MediaDriver process exited!");
        _process.OutputDataReceived += (sender, args) => _logger.LogInformation(args.Data);
        _process.ErrorDataReceived += (sender, args) => _logger.LogError(args.Data);

        if (_process.Start())
        {
            _isRunning.GetAndSet(true);
            _logger.LogInformation("MediaDriver process started");
            _process.WaitForExit();
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Stop();
        return Task.CompletedTask;
    }
    
    private void OnStopping()
    {
        _logger.LogInformation("Application is stopping...");
        Stop();
        // Other cleanup code here...
    }

    private void OnStopped()
    {
        _logger.LogInformation("Application has stopped.");
        // Other cleanup code here...
    }

    private void Stop()
    {
        if (_process != null && _isRunning.CompareAndSet(true, false))
        {
            _logger.LogInformation("stopping MediaDriver process");
            _process.Kill();
            _process.Dispose();
            _logger.LogInformation("MediaDriver process stopped!");
        }
        else
        {
            _logger.LogError("MediaDriver process already stoped");
        }
    }
}