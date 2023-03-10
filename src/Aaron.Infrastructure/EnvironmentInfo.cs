
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Aaron.Infrastructure;

public class EnvironmentInfo
{
    public static EnvironmentInfo Instance { get; } = Create();


#pragma warning disable CS8618
    public string EnvironmentName { get; init; }

    public string EntryAssemblyName { get; init; }

    public string EntryAssemblyVersion { get; init; }

    public string FrameworkDescription { get; init; }

    public string LocalTime => DateTime.UtcNow.ToString("o");

    public string MachineName { get; init; }

    public string OperatingSystemArchitecture { get; init; }

    public string OperatingSystemPlatform { get; init; }

    public string OperatingSystemVersion { get; init; }

    public string ProcessId { get; init; }

    public string ProcessArchitecture { get; init; }

    public string ProcessorCount { get; init; }
    public bool RunningInDocker { get; init; }


    public IReadOnlyDictionary<string, string> EnvironmaneVariables { get; init; }
    public IReadOnlyDictionary<string, string> Versions { get; init; }

#pragma warning restore CS8618

    private static EnvironmentInfo Create()
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        var framework = entryAssembly!
            .GetCustomAttribute<TargetFrameworkAttribute>()?
            .FrameworkName;
        var version = entryAssembly!
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;

        return new EnvironmentInfo
        {
            ProcessId = GetSafeString(() => Environment.ProcessId.ToString()),
            ProcessArchitecture = GetSafeString(() => RuntimeInformation.ProcessArchitecture.ToString()),
            OperatingSystemVersion = GetSafeString(() => RuntimeInformation.OSDescription),
            OperatingSystemPlatform = GetSafeString(() =>
            {
                var platform = OSPlatform.Create("Other Platform");
                var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                platform = isWindows ? OSPlatform.Windows : platform;
                var isOsx = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
                platform = isOsx ? OSPlatform.OSX : platform;
                var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
                platform = isLinux ? OSPlatform.Linux : platform;
                return platform.ToString();
            }),
            OperatingSystemArchitecture = GetSafeString(() => RuntimeInformation.OSArchitecture.ToString()),
            ProcessorCount = GetSafeString(() => Environment.ProcessorCount.ToString()),
            MachineName = GetSafeString(() => Environment.MachineName),
            FrameworkDescription = GetSafeString(() => framework!),
            EnvironmentName = GetEnvironment(),
            RunningInDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true",
            EntryAssemblyName = GetSafeString(() => entryAssembly?.GetName().Name ?? "unknown"),
            EntryAssemblyVersion = GetSafeString(() => version ?? "unknown"),
            EnvironmaneVariables = GetEnv().ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            Versions = GetVersions().ToDictionary(kvp => kvp.Name, kvp => kvp.Version)
        };

        string GetEnvironment()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

            if (!string.IsNullOrWhiteSpace(env))
                return env.ToLowerInvariant();

#if DEBUG
            return "debug";
#else
                return "release";
#endif
        }

        string GetSafeString(Func<string> action)
        {
            try
            {
                return action().Trim();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        IEnumerable<KeyValuePair<string, string>> GetEnv()
        {
            foreach (System.Collections.DictionaryEntry de in Environment.GetEnvironmentVariables())
            {
                var key = de.Key?.ToString() ?? string.Empty;
                if (key.StartsWith("ASPNETCORE_") || key.StartsWith("DOTNET_"))
                    yield return new KeyValuePair<string, string>(key, de!.Value?.ToString() ?? string.Empty);
            }
        }

        IEnumerable<(string Name, string Version)> GetVersions()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                    .Select(a => new
                    {
                        Name = a.GetName().Name,
                        Company = a.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company,
                        Version = a.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion.ToString()
                    })
                    .Where(x => x!.Name!.StartsWith("Aaron.") || x!.Name!.StartsWith("Akka.") || x.Company == "Rafal Piotrowski")
                    .Select(a => (a.Name, a.Version))!
            ;
        }
    }
}
