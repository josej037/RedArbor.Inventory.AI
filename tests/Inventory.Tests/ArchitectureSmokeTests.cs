using System.Reflection;

namespace Inventory.Tests;

public class ArchitectureSmokeTests
{
    [Fact]
    public void Domain_assembly_loads()
    {
        var assembly = Assembly.Load("Inventory.Domain");

        Assert.NotNull(assembly);
        Assert.Equal("Inventory.Domain", assembly.GetName().Name);
    }

    [Fact]
    public void Application_assembly_loads()
    {
        var assembly = Assembly.Load("Inventory.Application");

        Assert.NotNull(assembly);
        Assert.Equal("Inventory.Application", assembly.GetName().Name);
    }
}
