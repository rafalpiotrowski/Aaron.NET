
using System.Diagnostics;
using Akka.Cluster.Hosting;
using Akka.Configuration;
using Akka.Discovery.Azure;
using Akka.Hosting;
using Akka.Management;
using Akka.Management.Cluster.Bootstrap;
using Akka.Persistence.Azure;
using Akka.Persistence.Hosting;
using Akka.Remote.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Aaron.Configuration;

public static class AkkaConfiguration
{
    public static IServiceCollection ConfigureAaron(this IServiceCollection services, IConfiguration configuration,
        Action<AkkaConfigurationBuilder, IServiceProvider> additionalConfig)
    {
        var akkaSettings = configuration.GetRequiredSection("AkkaSettings").Get<AkkaSettings>();
        Debug.Assert(akkaSettings != null, nameof(akkaSettings) + " != null");

        services.AddSingleton(akkaSettings);
        services.AddLogging(config =>
        {
            config.AddConsole();
        });

        return services.AddAkka(akkaSettings.ActorSystemName, (builder, sp) =>
        {
            builder.ConfigureActorSystem(sp);
            additionalConfig(builder, sp);
        });
    }

    public static AkkaConfigurationBuilder ConfigureActorSystem(this AkkaConfigurationBuilder builder,
        IServiceProvider sp)
    {
        var settings = sp.GetRequiredService<AkkaSettings>();

        return builder
            .ConfigureLoggers(configBuilder =>
            {
                configBuilder.LogConfigOnStart = settings.LogConfigOnStart;
                // replace
                configBuilder.AddLoggerFactory();
                // with our own logging factory for better handling messages
                configBuilder.ClearLoggers();
                configBuilder.AddTypedLoggerFactory();
            })
            .ConfigureNetwork(sp)
            .ConfigurePersistence(sp);
    }

    public static AkkaConfigurationBuilder ConfigureNetwork(this AkkaConfigurationBuilder builder,
        IServiceProvider serviceProvider)
    {
        var settings = serviceProvider.GetRequiredService<AkkaSettings>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        if (!settings.UseClustering)
            return builder;

        var b = builder
            .WithRemoting(settings.RemoteOptions);

        if (settings.AkkaManagementOptions is { Enabled: true })
        {
            // need to delete seed-nodes so Akka.Management will take precedence
            var clusterOptions = settings.ClusterOptions;
            clusterOptions.SeedNodes = Array.Empty<string>();

            b = b
                .WithClustering(clusterOptions)
                .WithAkkaManagement(hostName: settings.AkkaManagementOptions.Hostname,
                    settings.AkkaManagementOptions.Port)
                .WithClusterBootstrap(serviceName: settings.AkkaManagementOptions.ServiceName,
                    portName: settings.AkkaManagementOptions.PortName,
                    requiredContactPoints: settings.AkkaManagementOptions.RequiredContactPointsNr);

            switch (settings.AkkaManagementOptions.DiscoveryMethod)
            {
                case DiscoveryMethod.Kubernetes:
                    break;
                case DiscoveryMethod.AwsEcsTagBased:
                    break;
                case DiscoveryMethod.AwsEc2TagBased:
                    break;
                case DiscoveryMethod.AzureTableStorage:
                {
                    var connectionStringName = configuration.GetSection("AzureStorageSettings")
                        .Get<AzureStorageSettings>()?.ConnectionStringName;
                    Debug.Assert(connectionStringName != null, nameof(connectionStringName) + " != null");
                    var connectionString = configuration.GetConnectionString(connectionStringName);

                    b = b.WithAzureDiscovery(options =>
                    {
                        options.ServiceName = settings.AkkaManagementOptions.ServiceName;
                        options.ConnectionString = connectionString;
                    });
                    break;
                }
                case DiscoveryMethod.Config:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        else
        {
            b = b.WithClustering(settings.ClusterOptions);
        }

        return b;
    }

    public static Config GetPersistenceHocon(string connectionString)
    {
        return $@"
            akka.persistence {{
                journal {{
                    plugin = ""akka.persistence.journal.azure-table""
                    azure-table {{
                        class = ""Akka.Persistence.Azure.Journal.AzureTableStorageJournal, Akka.Persistence.Azure""
                        connection-string = ""{connectionString}""
                    }}
                }}
                 snapshot-store {{
                     plugin = ""akka.persistence.snapshot-store.azure-blob-store""
                     azure-blob-store {{
                        class = ""Akka.Persistence.Azure.Snapshot.AzureBlobSnapshotStore, Akka.Persistence.Azure""
                        connection-string = ""{connectionString}""
                    }}
                }}
            }}";
    }

    public static AkkaConfigurationBuilder ConfigurePersistence(this AkkaConfigurationBuilder builder,
        IServiceProvider serviceProvider)
    {
        var settings = serviceProvider.GetRequiredService<AkkaSettings>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        switch (settings.PersistenceMode)
        {
            case PersistenceMode.InMemory:
                return builder.WithInMemoryJournal().WithInMemorySnapshotStore();
            case PersistenceMode.Azure:
            {
                var connectionStringName = configuration.GetSection("AzureStorageSettings")
                    .Get<AzureStorageSettings>()?.ConnectionStringName;
                Debug.Assert(connectionStringName != null, nameof(connectionStringName) + " != null");
                var connectionString = configuration.GetConnectionString(connectionStringName);
                Debug.Assert(connectionString != null, nameof(connectionString) + " != null");

                // todo: switch to builder.WithAzurePersistence() 
                // return builder.WithAzurePersistence(); // doesn't work right now
                return builder.AddHocon(
                    GetPersistenceHocon(connectionString).WithFallback(AzurePersistence.DefaultConfig),
                    HoconAddMode.Append);
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(settings.PersistenceMode), settings.PersistenceMode,
                    "not supported");
        }
    }
}