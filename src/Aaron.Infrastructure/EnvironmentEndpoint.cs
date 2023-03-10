using System.Net.Mime;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Aaron.Infrastructure;

public static class EnvironmentEndpoint
{
    public const string DefaultPath = "/_env";

    public static IEndpointRouteBuilder MapEnv(this IEndpointRouteBuilder endpoints, string path = DefaultPath)
    {
        var envInfo = JsonSerializer.SerializeToUtf8Bytes(
            EnvironmentInfo.Instance,
            new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }
        );

        endpoints.MapGet(path, async ctx =>
        {
            ctx.Response.ContentType = MediaTypeNames.Application.Json;
            await ctx.Response.BodyWriter.WriteAsync(envInfo);
        });

        return endpoints;
    }
}