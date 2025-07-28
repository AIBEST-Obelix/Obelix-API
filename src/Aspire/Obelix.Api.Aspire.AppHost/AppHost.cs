using Obelix.Api.Aspire.AppHost.Extension;
using Obelix.Api.Aspire.AppHost.Helpers;

#pragma warning disable ASPIREHOSTINGPYTHON001

var builder = DistributedApplication.CreateBuilder(args);

bool pythonInited = false;
try
{
    await PythonInit.SetupPythonEnvironmentAsync();
    pythonInited = true;
}
catch (Exception ex)
{
    Console.WriteLine($"Python environment setup failed: {ex.Message}");
}

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
    .WithReference(requestsDb)
    .WithReference(identityService)
    .WithReference(itemsService)
    .WithReference(rabbitMq)
    .WithEnvironment("JWT")
    .WithExternalHttpEndpoints()
    .WaitFor(requestsMigrationService);

if (!pythonInited)
{
    Console.WriteLine("Python environment is not initialized, skipping Python services.");
    
    builder
        .AddProject<Projects.Obelix_Api_Gateway_WebHost>("api-gateway")
        .WithReference(identityService)
        .WithReference(itemsService)
        .WithReference(requestsService);
}
else
{
    var analyzeService = builder
        .AddPythonApp("analyze-service", "../../Services/Obelix.Api.Services.Analyze", "main.py")
        .WithHttpEndpoint(targetPort: 8111, port: 8111, isProxied: false)
        .WithExternalHttpEndpoints()
        .WithOtlpExporter()
        .WithReference(rabbitMq);

    builder
        .AddProject<Projects.Obelix_Api_Gateway_WebHost>("api-gateway")
        .WithReference(identityService)
        .WithReference(itemsService)
        .WithReference(requestsService)
        .WithReference(analyzeService);
}

builder.Build().Run();