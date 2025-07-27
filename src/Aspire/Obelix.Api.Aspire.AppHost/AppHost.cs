using Obelix.Api.Aspire.AppHost.Extension;

var builder = DistributedApplication.CreateBuilder(args);

var rabbitMqUsername = builder.AddParameter("database-username");
var rabbitMqPassword = builder.AddParameter("database-password", secret: true);

// Database
var identityDb = builder.AddPostgres("identity-db-server", password: rabbitMqPassword)
    .WithDataVolume()
    .AddDatabase("identity-db");

var itemsDb = builder.AddPostgres("items-db-server", password: rabbitMqPassword)
    .WithDataVolume()
    .AddDatabase("items-db");

var requestsDb = builder.AddPostgres("requests-db-server", password: rabbitMqPassword)
    .WithDataVolume()
    .AddDatabase("requests-db");

var rabbitMq = builder.AddRabbitMQ(
        "Obelix-eventbus",
        rabbitMqUsername,
        rabbitMqPassword,
        5849)
    .WithManagementPlugin();

// Migration services
var identityMigrationService = builder.AddProject<Projects.Obelix_Api_Services_Identity_MigrationServices>("identity-migration-service")
    .WithReference(identityDb)
    .WithEnvironment("SuperAdmin");

var itemsMigrationService = builder
    .AddProject<Projects.Obelix_Api_Services_Items_MigrationServices>("items-migration-service")
    .WithReference(itemsDb);

var requestsMigrationService = builder
    .AddProject<Projects.Obelix_Api_Services_Requests_MigrationServices>("requests-migration-service")
    .WithReference(requestsDb);

// Services
var identityService = builder
    .AddProject<Projects.Obelix_Api_Services_Identity_WebHost>("identity-service")
    .WithReference(identityDb)
    .WithReference(rabbitMq)
    .WithEnvironment("JWT")
    .WithExternalHttpEndpoints()
    .WaitFor(identityMigrationService);

var itemsService = builder
    .AddProject<Projects.Obelix_Api_Services_Items_WebHost>("items-service")
    .WithReference(itemsDb)
    .WithReference(rabbitMq)
    .WithEnvironment("JWT")
    .WithExternalHttpEndpoints()
    .WaitFor(itemsMigrationService);

var requestsService = builder
    .AddProject<Projects.Obelix_Api_Services_Requests_WebHost>("requests-service")
    .WithReference(identityService)
    .WithReference(itemsService)
    .WithReference(rabbitMq)
    .WithEnvironment("JWT")
    .WithExternalHttpEndpoints()
    .WaitFor(requestsMigrationService);

builder
    .AddProject<Projects.Obelix_Api_Gateway_WebHost>("api-gateway")
    .WithReference(identityService)
    .WithReference(itemsService);

builder.Build().Run();