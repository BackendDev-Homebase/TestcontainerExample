// Prerequisites: Installation and start of Docker Desktop (Azure Build Server apparently works without it)

// - Context must have a ctor with DbContextOptions<>

// - add NuGet "Testcontainers.MsSql"
//   Testcontainers for .NET is a library to support tests with throwaway instances of Docker containers for all compatible .NET Standard versions.

using FluentAssertions;

namespace TestcontainerExample.Tests;

public class IntegrationTests : IntegrationTestBase
{
    [Fact]
    public async void Test()
    {
        // Arrange
        var e1 = CreateEntity();
        var e2 = CreateEntity();

        Context.Entities.Add(e1);
        Context.Entities.Add(e2);
        await Context.SaveChangesAsync();

        // Act
        var count = Context.Entities.Count();

        // Assert
        count.Should().Be(2);
    }

    private Entity CreateEntity()
    {
        return new Entity
        {
            Text = Guid.NewGuid().ToString()
        };
    }
}