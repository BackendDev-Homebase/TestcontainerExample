using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace TestcontainerExample.Tests;

public class IntegrationTestBase : IAsyncLifetime
{
    public MsSqlContainer Database { get; private set; }
    public Context? Context { get; private set; }

    public IntegrationTestBase()
    {
        Database = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithWaitStrategy(Wait
                .ForUnixContainer()
                .AddCustomWaitStrategy(new CustomWaitUntil()) // Custom Wait Strategy for Azure Build Server
             )
            .Build();
    }

    public async Task InitializeAsync()
    {
        await Database.StartAsync();
        var options = new DbContextOptionsBuilder<Context>()
                    .UseSqlServer(Database.GetConnectionString(), optionsBuilder =>
                        optionsBuilder
                            .MigrationsAssembly("TestcontainerExample")
                            .EnableRetryOnFailure()) // Exception on Azure Build Server: System.InvalidOperationException : An exception has been raised that is likely due to a transient failure. Consider enabling transient error resiliency by adding 'EnableRetryOnFailure' to the 'UseSqlServer' call.
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
                    .Options;
        Context = new Context(options);
        await Context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await Database.DisposeAsync();
    }
}

public sealed class CustomWaitUntil : IWaitUntil
{
    public CustomWaitUntil()
    { }

    public Task<bool> UntilAsync(IContainer container)
    {
        var secondsToWait = 5;
        return Task.FromResult(container.CreatedTime.AddSeconds(secondsToWait) <= DateTime.UtcNow);
    }
}