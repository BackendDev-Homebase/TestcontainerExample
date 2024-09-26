// Prerequisites: Installation and start of Docker Desktop (Azure Build Server apparently works without it)

// - Context must have a ctor with DbContextOptions<>

// - add NuGet "Testcontainers.MsSql"
//   Testcontainers for .NET is a library to support tests with throwaway instances of Docker containers for all compatible .NET Standard versions.

using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace TestcontainerExample.Tests;


public class IntegrationTests : IAsyncLifetime, IClassFixture<IntegrationTestBase>
{
    private readonly IntegrationTestBase _fixture;

    public IntegrationTests(IntegrationTestBase fixture)
    {
        _fixture = fixture;
    }
    public Task InitializeAsync()
    {
        // is executed before each test
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        // is executed after each test
        return _fixture.Context!.Entities.ExecuteDeleteAsync();
    }


    [Fact]
    public async void SomeTest()
    {
        await TestContent();
    }

    [Fact]
    public async void OtherTest()
    {
        await TestContent();
    }

    private async Task TestContent()
    {
        var count = _fixture.Context!.Entities.Count();
        count.Should().Be(0);

        // Arrange
        var e1 = CreateEntity();
        var e2 = CreateEntity();

        _fixture.Context.Entities.Add(e1);
        _fixture.Context.Entities.Add(e2);
        await _fixture.Context.SaveChangesAsync();

        // Act
        count = _fixture.Context.Entities.Count();

        // Assert
        count.Should().Be(2);
    }

    private static Entity CreateEntity()
    {
        return new Entity
        {
            Text = Guid.NewGuid().ToString()
        };
    }
}