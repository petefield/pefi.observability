using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Logs;
using System.Reflection;

namespace pefi.observability
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPefiObservability(this IServiceCollection services)
        {

            var serviceName = Assembly.GetEntryAssembly().GetName().Name;
            var serviceVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();

            services.AddOpenTelemetry()
                .ConfigureResource(r => r.AddService(serviceName, null, serviceVersion))
                .WithTracing(t => t
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter(o => o.Endpoint = new Uri("http://localhost:4317"))
                    .AddConsoleExporter())
                .WithLogging(l => l.AddConsoleExporter()
                    .AddOtlpExporter(o => {
                        o.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                        o.Endpoint = new Uri("http://localhost:4317");
                    })
                 )
                .WithMetrics(m => m
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddProcessInstrumentation()
                    .AddOtlpExporter(o => o.Endpoint = new Uri("http://localhost:4317")));

        }
    }
}
