using FluentAssertions;
using TestcontainerExample.Tests.Infrastructure;

namespace TestcontainerExample.Tests;

public class IntegrationTestsClass1 : IntegrationTestBase
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
        var e1 = CreateEntity();
        var e2 = CreateEntity();

        _context.Entities.Add(e1);
        _context.Entities.Add(e2);
        await _context.SaveChangesAsync();

        // Act
        count = _context.Entities.Count();

        // Assert
        count.Should().Be(2);
    }

    internal static Entity CreateEntity()
    {
        return new Entity
        {
            Text = Guid.NewGuid().ToString()
        };
    }
}