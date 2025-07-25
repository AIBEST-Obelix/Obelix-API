// Wait for ten seconds to allow the database to start
// TODO: Remove in PROD

using Obelix.Api.Services.Requests.Data.Data;
using Obelix.Api.Services.Requests.MigrationServices;

Thread.Sleep(10000);

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddNpgsqlDbContext<ApplicationDbContext>("requests-db");

var host = builder.Build();
host.Run();