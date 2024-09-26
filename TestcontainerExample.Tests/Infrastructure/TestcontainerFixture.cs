// Prerequisites: Installation and start of Docker Desktop (Azure Build Server apparently works without it)

// - Context must have a ctor with DbContextOptions<>

// - add NuGet "Testcontainers.MsSql"
//   Testcontainers for .NET is a library to support tests with throwaway instances of Docker containers for all compatible .NET Standard versions.

using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace TestcontainerExample.Tests.Infrastructure;

public class TestcontainerFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _database;
    public static Context? Context { get; private set; }

    public TestcontainerFixture()
    {
        // is executed once before all tests run
        _database = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithWaitStrategy(Wait
                .ForUnixContainer()
                .AddCustomWaitStrategy(new CustomWaitUntil()) // Custom Wait Strategy for Azure Build Server
             )
            .Build();
    }

    public async Task InitializeAsync()
    {
        // is executed once before all tests run - after the ctor
        await _database.StartAsync();
        var options = new DbContextOptionsBuilder<Context>()
                    .UseSqlServer(_database.GetConnectionString(), optionsBuilder =>
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
        // is executed once after all tests run
        await _database.DisposeAsync();
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