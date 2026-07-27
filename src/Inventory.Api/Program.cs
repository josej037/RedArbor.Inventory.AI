using Inventory.Api.ExceptionHandling;
using Inventory.Api.Swagger;
using Inventory.Application;
using Inventory.Infrastructure;
using Inventory.Infrastructure.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddControllers();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddInventorySwagger();

var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseInventorySwagger();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await SeedDemoDataAsync(app.Services);

app.Run();

static async Task SeedDemoDataAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DemoDataSeeder>();
    await seeder.SeedAsync();
}
