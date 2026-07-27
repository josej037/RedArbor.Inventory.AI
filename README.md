# RedArbor Inventory AI

ASP.NET Core inventory REST API built with Clean Architecture, CQRS-style EF Core reads / Dapper writes, JWT Bearer authentication, and Docker Compose for local runs.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for Compose)

## Solution structure

```text
Inventory.Api            # HTTP host, controllers, Swagger
Inventory.Application    # Business rules, DTOs, repository interfaces
Inventory.Domain         # Entities and enums
Inventory.Infrastructure # EF Core, Dapper, JWT, repositories, seed
Inventory.Tests          # Domain and Application unit tests
```

Reads use **Entity Framework Core**; writes (INSERT/UPDATE/DELETE) use **Dapper**. Controllers stay thin and never use `DbContext` directly.

## Installation

```bash
dotnet restore Inventory.sln
dotnet build Inventory.sln
```

## Configuration

Settings are loaded from `appsettings.json` / `appsettings.Development.json` and overridden by environment variables (including Docker Compose).

| Setting | Env var | Purpose |
|---------|---------|---------|
| `ConnectionStrings:InventoryDb` | `ConnectionStrings__InventoryDb` | SQL Server connection |
| `Jwt:Issuer` | `Jwt__Issuer` | JWT issuer |
| `Jwt:Audience` | `Jwt__Audience` | JWT audience |
| `Jwt:Key` | `Jwt__Key` | Signing key (≥ 32 characters) |
| `Jwt:ExpirationMinutes` | `Jwt__ExpirationMinutes` | Token lifetime |
| `Auth:Username` | `Auth__Username` | Demo login username |
| `Auth:Password` | `Auth__Password` | Demo login password |

Development defaults live in `src/Inventory.Api/appsettings.Development.json`.

## Docker (recommended)

1. Copy the sample env file and adjust if needed:

   ```bash
   cp .env.example .env
   ```

2. Build and start API + SQL Server:

   ```bash
   docker compose up --build -d
   ```

3. Open Swagger: [http://localhost:8080/swagger](http://localhost:8080/swagger)

The `api` service waits until `sqlserver` is healthy, applies pending EF migrations, then runs the demo seeder when Categories/Products are empty. SQL data persists in the `sqlserver_data` volume.

Stop:

```bash
docker compose down
```

## Local API (SQL via Compose)

1. Start only SQL Server (or the full stack):

   ```bash
   docker compose up -d sqlserver
   ```

2. Ensure `appsettings.Development.json` points at `localhost,1433` with the same SA password as `.env`.

3. Run the API (migrations apply at startup before seed):

   ```bash
   dotnet run --project src/Inventory.Api
   ```

Optional (if you need to apply migrations without starting the API):

```bash
dotnet ef database update --project src/Inventory.Infrastructure --startup-project src/Inventory.Api
```

## Swagger and authentication

1. Open `/swagger`.
2. Call `POST /api/auth/login` with the demo credentials.
3. Click **Authorize** and enter `Bearer <accessToken>` (or paste the token alone if the UI already prefixes Bearer).
4. Exercise Categories, Products, Entries, Exits, and Movements.

Protected inventory endpoints return **401** without a token. Business rule violations return **400**; missing resources return **404**.

## Demo credentials

| Field | Value |
|-------|-------|
| Username | `demo` |
| Password | `Demo123!` |

These match Development config and `.env.example`. Change them via `Auth__*` / `Jwt__*` before sharing an environment.

## Architecture overview

- **Domain** — inventory entities (Category, Product, Entry, Exit, Movement).
- **Application** — services and validation (stock rules, delete guards, auth login).
- **Infrastructure** — SQL Server via EF (reads) and Dapper (writes), JWT generation/validation, demo seed.
- **API** — REST controllers + global exception handling + Swagger.

See ADRs under `docs/adr/` and the Milestone 5 review in [docs/03-Architecture-Review.md](docs/03-Architecture-Review.md). AI-oriented project notes: [docs/README_AI.md](docs/README_AI.md).

## Tests

```bash
dotnet test
```

Unit tests cover Domain entities and Application business rules (xUnit, Moq, FluentAssertions).
