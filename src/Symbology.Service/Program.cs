using Akka.HealthCheck.Hosting;
using Akka.HealthCheck.Hosting.Web;
using Aaron.Configuration;
using Aaron.Infrastructure;
using Aaron.Infrastructure.Monitoring;
using Aaron.Infrastructure.Serialization;
using Aeron.MediaDriver;
using Symbology.Actors;
using Symbology.Serialization;
using Symbology.Service.Configuration;

var builder = WebApplication.CreateBuilder(args)
    .AddTelemetry();

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

/*
 * CONFIGURATION SOURCES
 */
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{environment}.json", optional: true)
    .AddEnvironmentVariables();

// Add services to the container.
builder.Services.WithAkkaHealthCheck(HealthCheckType.All);
builder.Services.ConfigureAaron(builder.Configuration, (akkaConfigurationBuilder, serviceProvider) =>
{
    // we configure instrumentation separately from the internals of the ActorSystem
    akkaConfigurationBuilder.ConfigurePetabridgeCmd(cmd =>
        {
            cmd.ConfigureSymbologyPetabridgeCmd();
        })
        .WithWebHealthCheck(serviceProvider)
        .WithTraceableDispatcher(dispatcher => 
            dispatcher.AddTraceableAssemblyOf<InstrumentActor>())
        .AddAaronContractsSerialization()
        .AddSymbologySerialization()
        .ConfigureSymbologyActors(serviceProvider);
});

builder.Services.AddHostedService<AeronMediaDriverBackgroundService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName.Equals("Azure"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapAkkaHealthCheckRoutes(optionConfigure: (_, opt) =>
{
    // Use a custom response writer to output a json of all reported statuses
    opt.ResponseWriter = Helper.JsonResponseWriter;
}); // needed for Akka.HealthCheck
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => app.Environment.ApplicationName);

//add metrics scrapping end point default: /metrics
app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.MapEnv();

app.Run();

/// <summary>
/// Required for integration tests
/// </summary>
public partial class Program
{
    
}