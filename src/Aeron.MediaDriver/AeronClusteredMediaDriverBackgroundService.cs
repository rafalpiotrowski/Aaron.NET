using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aeron.MediaDriver;

public sealed class AeronMediaDriverBackgroundService : IHostedService
{
    private readonly ILogger<AeronMediaDriverBackgroundService> _logger;

    public AeronMediaDriverBackgroundService(ILogger<AeronMediaDriverBackgroundService> logger)
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
                Arguments = "-cp ./aeron/media-driver.jar --illegal-access=warn io.aeron.driver.MediaDriver",
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