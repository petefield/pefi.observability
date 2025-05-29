using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Logs;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace pefi.observability
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPefiLogging(this ILoggingBuilder l)
        {
            var serviceName = Assembly.GetEntryAssembly().GetName().Name;

            var resource = ResourceBuilder.CreateDefault().AddService(serviceName);

            l.AddOpenTelemetry(loggerOptions =>
            {
                loggerOptions.SetResourceBuilder(resource);
                loggerOptions.IncludeFormattedMessage = true;
                loggerOptions.IncludeScopes = true;
                loggerOptions.ParseStateValues = true;
                loggerOptions.AddConsoleExporter();
            });
        }

        public static void AddPefiObservability(this IServiceCollection services, string endpoint)
        {

            var serviceName = Assembly.GetEntryAssembly().GetName().Name;
            var serviceVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();

            services.AddOpenTelemetry()
                .ConfigureResource(r => r.AddService(serviceName, null, serviceVersion))
                .WithTracing(t => t
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(o => o.Endpoint = new Uri(endpoint))
                    .AddConsoleExporter())
                .WithLogging(l => l
                    .AddConsoleExporter()
                    
                    .AddOtlpExporter(o => {
                        o.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                        o.Endpoint = new Uri(endpoint);
                    })
                 )
                .WithMetrics(m => m
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddProcessInstrumentation()
                    .AddOtlpExporter(o => o.Endpoint = new Uri(endpoint)));

        }
    }
}
