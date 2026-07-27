using System.Data.Common;
using Inventory.Application.Abstractions.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Inventory.Infrastructure.Persistence;

public class SqlConnectionFactory(IConfiguration configuration) : ISqlConnectionFactory
{
    private readonly string _connectionString = configuration.GetConnectionString("InventoryDb")
        ?? throw new InvalidOperationException("Connection string 'InventoryDb' is not configured.");

    public DbConnection CreateConnection() => new SqlConnection(_connectionString);
}
