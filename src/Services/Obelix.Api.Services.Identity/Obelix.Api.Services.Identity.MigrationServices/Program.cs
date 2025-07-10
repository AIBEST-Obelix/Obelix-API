using Obelix.Api.Services.Identity.Services;
using Obelix.Api.Services.Identity.Data.Data;
using Obelix.Api.Services.Identity.Data.Models.Identity;
using Obelix.Api.Services.Identity.MigrationServices;
using Obelix.Api.Services.Identity.Services;
using Obelix.Api.Services.Identity.WebHost.Profiles;
using Microsoft.AspNetCore.Identity;


// Wait for ten seconds to allow the database to start
// TODO: Remove in PROD
Thread.Sleep(10000);

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();
builder.Services.AddServices();

builder.Services
    .AddIdentity<User, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.AddNpgsqlDbContext<ApplicationDbContext>("identity-db");

var host = builder.Build();

host.Run();