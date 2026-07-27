using FluentAssertions;
using Inventory.Domain.Entities;
using Inventory.Domain.Exceptions;

namespace Inventory.Tests.Domain;

public class CategoryTests
{
    [Fact]
    public void Create_with_valid_name_succeeds()
    {
        var category = new Category("Electronics", "Devices");

        category.Name.Should().Be("Electronics");
        category.Description.Should().Be("Devices");
        category.CreatedAtUtc.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_with_invalid_name_throws(string? name)
    {
        var act = () => new Category(name!);

        act.Should().Throw<DomainException>();
    }
}
