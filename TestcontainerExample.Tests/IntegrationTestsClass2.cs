// Prerequisites: Installation and start of Docker Desktop (Azure Build Server apparently works without it)

// - Context must have a ctor with DbContextOptions<>

// - add NuGet "Testcontainers.MsSql"
//   Testcontainers for .NET is a library to support tests with throwaway instances of Docker containers for all compatible .NET Standard versions.

using FluentAssertions;
using TestcontainerExample.Tests.Infrastructure;

namespace TestcontainerExample.Tests;

public class IntegrationTestsClass2 : IntegrationTestBase
{
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
        var count = _context!.Entities.Count();
        count.Should().Be(0);

        // Arrange
        var e1 = IntegrationTestsClass1.CreateEntity();
        var e2 = IntegrationTestsClass1.CreateEntity();

        _context.Entities.Add(e1);
        _context.Entities.Add(e2);
        await _context.SaveChangesAsync();

        // Act
        count = _context.Entities.Count();

        // Assert
        count.Should().Be(2);
    }
}