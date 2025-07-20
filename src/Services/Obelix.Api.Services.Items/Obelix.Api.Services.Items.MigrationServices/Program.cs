using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Obelix.Api.Services.Identity.MigrationServices;
using Obelix.Api.Services.Items.Data.Data;


// Wait for ten seconds to allow the database to start
// TODO: Remove in PROD
Thread.Sleep(10000);

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.AddNpgsqlDbContext<ApplicationDbContext>("items-db");

var host = builder.Build();
host.Run();