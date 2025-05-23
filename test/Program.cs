using pefi.observability;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPefiObservability();
builder.Services.AddHttpClient();

var app = builder.Build();

app.MapGet("/", async (ILogger<Program> logger , IHttpClientFactory httpClientFactory) => {
    logger.LogInformation("Hello from OpenTelemetry + Loki! v2 {name}", Guid.NewGuid());
    var c = httpClientFactory.CreateClient();
    var r = await c.GetAsync("https://www.google.com");
    return "Hello from OpenTelemetry!";
});

app.Run();