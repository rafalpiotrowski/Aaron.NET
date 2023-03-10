
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Aaron.Infrastructure;

public static class TelemetryExtensions
{
    public static WebApplicationBuilder AddTelemetry(this WebApplicationBuilder builder)
    {
        var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name;
        
        var resource = ResourceBuilder.CreateDefault()
            .AddService(assemblyName!, serviceInstanceId: $"{Dns.GetHostName()}");

        // enables OpenTelemetry for ASP.NET / .NET Core
        builder.Services.AddOpenTelemetry().WithTracing(tracerProviderBuilder =>
        {
            tracerProviderBuilder
                .SetResourceBuilder(resource)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddSource(TelemetryConstants.TelemetryNamespace)
                .AddSource(assemblyName);

            var jaegerAgentHost = builder.Configuration.GetValue<string>("JAEGER_AGENT_HOST");
            if (!string.IsNullOrEmpty(jaegerAgentHost) && !string.IsNullOrWhiteSpace(jaegerAgentHost))
            {
                Console.WriteLine($"AddJaegerExporter; JAEGER_AGENT_HOST: {jaegerAgentHost}");
                tracerProviderBuilder.AddJaegerExporter(opt =>
                {
                    opt.AgentHost = jaegerAgentHost;
                });
            }
            else
                Console.WriteLine($"to AddJaegerExporter define JAEGER_AGENT_HOST");
        });

        builder.Services.AddOpenTelemetry().WithMetrics(meterProviderBuilder =>
        {
            meterProviderBuilder
                .SetResourceBuilder(resource)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddProcessInstrumentation()
                .AddPrometheusExporter(o => o.ScrapeResponseCacheDurationMilliseconds = 3000)
                .AddMeter(TelemetryConstants.TelemetryNamespace)
                .AddView(instrument =>
                {
                    if (instrument.Meter.Name == TelemetryConstants.TelemetryNamespace
                        && instrument.Unit is not null
                        && TelemetryConstants.DurationUnits.Contains(instrument.Unit))
                    {
                        return new ExplicitBucketHistogramConfiguration
                        {
                            Boundaries = new double[] { 5, 10, 25, 50, 100, 250, 500, 1000, 2500, 5000, 10000, 50000, 100000, 250000, 500000, 1000000 }
                        };
                    }

                    return null;
                });
        });

        return builder;
    }
}
