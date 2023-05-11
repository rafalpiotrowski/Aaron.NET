using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aeron.MediaDriver;

public sealed class AeronClusteredMediaDriverBackgroundService : IHostedService
{
    private readonly ILogger<AeronClusteredMediaDriverBackgroundService> _logger;

    public AeronClusteredMediaDriverBackgroundService(ILogger<AeronClusteredMediaDriverBackgroundService> logger)
    {
        _logger = logger;
    }
    
    private Process? _process;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "java",
                Arguments = "-cp ./aeron/media-driver.jar " + 
                            "-Daeron.cluster.ingress.channel=aeron:udp?endpoint=localhost:9010 " +
                            "-Daeron.archive.control.channel=aeron:udp?endpoint=localhost:8010 " + 
                            "-Daeron.archive.replication.channel=aeron:udp?endpoint=localhost:0 " + 
                            "-Daeron.cluster.replication.channel=aeron:udp?endpoint=localhost:9011 " +
                            "-Daeron.cluster.members=\"0,localhost:20000,localhost:20001,localhost:20002,localhost:0,localhost:8010\" " +
                            "io.aeron.cluster.ClusteredMediaDriver",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        _process.Start();

        // Optional: log process output
        _process.OutputDataReceived += (sender, args) => _logger.LogInformation(args.Data);
        _process.ErrorDataReceived += (sender, args) => _logger.LogError(args.Data);
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (!(_process?.HasExited ?? true))
        {
            _process?.CancelOutputRead();
            _process?.CancelErrorRead();
            _process?.Kill();
        }
        _process?.Dispose();

        return Task.CompletedTask;
    }
}