using Microsoft.EntityFrameworkCore;

namespace TestcontainerExample.Tests.Infrastructure;

[Collection("IntegrationTest")]
public class IntegrationTestBase : IAsyncLifetime
{
    protected readonly Context _context = TestcontainerFixture.Context!;

    public Task InitializeAsync()
    {
        // is executed before each test
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        // is executed after each test
        return _context.Entities.ExecuteDeleteAsync();
    }
}
