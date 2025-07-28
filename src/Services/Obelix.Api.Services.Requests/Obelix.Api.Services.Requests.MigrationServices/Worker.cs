using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Obelix.Api.Services.Requests.Data.Data;
using OpenTelemetry.Trace;

namespace Obelix.Api.Services.Requests.MigrationServices;

public class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    internal const string ActivitySourceName = "Migrations";
    private static readonly ActivitySource s_activitySource = new(ActivitySourceName);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = s_activitySource.StartActivity("Migrating database", ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
            //var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            await EnsureDatabaseAsync(dbContext, cancellationToken);
            await RunMigrationAsync(dbContext, cancellationToken);
            //await SeedDataAsync(dbContext, authService, configuration, cancellationToken);
        }
        catch (Exception ex)
        {
            activity?.RecordException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task EnsureDatabaseAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            if (!await dbCreator.ExistsAsync(cancellationToken))
            {
                await dbCreator.CreateAsync(cancellationToken);
            }
        });
    }

    private static async Task RunMigrationAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Entity Framework execution strategies already handle transactions internally
            await dbContext.Database.MigrateAsync(cancellationToken);
        });
    }
    
    private static async Task SeedDataAsync(ApplicationDbContext dbContext,
        IConfiguration configuration, CancellationToken cancellationToken)
    {
        /*
       if (!dbContext.Users.Any())
       {
           AdminIM admin = new()
           {
               UserName = configuration.GetValue<string>("Admin:Username"),
               Password = configuration.GetValue<string>("Admin:Password"),
           };

           await authService.CreateAdminAsync(admin);
       }

       var strategy = dbContext.Database.CreateExecutionStrategy();
       await strategy.ExecuteAsync(async () =>
       {
           // Seed the database
           await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
           await dbContext.Tickets.AddAsync(firstTicket, cancellationToken);
           await dbContext.SaveChangesAsync(cancellationToken);
           await transaction.CommitAsync(cancellationToken);
       });*/
    }
}
