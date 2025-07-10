using Obelix.Api.Aspire.AppHost.Extension;

var builder = DistributedApplication.CreateBuilder(args);

var rabbitMqUsername = builder.AddParameter("database-username");
var rabbitMqPassword = builder.AddParameter("database-password", secret: true);

// Database
var identityDb = builder.AddPostgres("identity-db-server", password: rabbitMqPassword)
    .WithDataVolume()
    .AddDatabase("identity-db");

var rabbitMq = builder.AddRabbitMQ(
        "Obelix-eventbus",
        rabbitMqUsername,
        rabbitMqPassword,
        5849)
    .WithManagementPlugin();

// Migration services
var migrationService = builder.AddProject<Projects.Obelix_Api_Services_Identity_MigrationServices>("identity-migration-service")
    .WithReference(identityDb)
    .WithEnvironment("SuperAdmin");

// Services
var identityService = builder
    .AddProject<Projects.Obelix_Api_Services_Identity_WebHost>("identity-service")
    .WithReference(identityDb)
    .WithReference(rabbitMq)
    .WithEnvironment("JWT")
    .WaitFor(migrationService);

builder
    .AddProject<Projects.Obelix_Api_Gateway_WebHost>("api-gateway")
    .WithReference(identityService);

builder.Build().Run();