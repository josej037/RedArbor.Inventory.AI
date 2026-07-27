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
| `GitHub:ClientId` | `GitHub__ClientId` | GitHub OAuth App client ID |
| `GitHub:ClientSecret` | `GitHub__ClientSecret` | GitHub OAuth App client secret |
| `GitHub:RedirectUri` | `GitHub__RedirectUri` | Callback URL registered in GitHub |

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

3. Open Swagger: [http://localhost:8080/index.html](http://localhost:8080/index.html)

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

This API uses **GitHub OAuth2 Authorization Code** only to prove identity, then issues its own JWT (existing `JwtTokenGenerator`). Protected endpoints accept **only** that app JWT — not a GitHub access token.

### GitHub OAuth App setup

This API uses GitHub OAuth2 Authorization Code flow.

The project includes a preconfigured GitHub OAuth application for technical evaluation.

No GitHub OAuth registration is required.

Authentication flow:

1. Open:

   http://localhost:8080/api/auth/login

2. Sign in with GitHub.

3. GitHub redirects to:

   http://localhost:8080/api/auth/callback

4. The API exchanges the GitHub identity for its own JWT token.

5. Use the JWT token in Swagger:

   Authorize → Bearer <token>

Protected endpoints require this JWT.

### Login flow

1. Open `/swagger` (or browse `GET /api/auth/login`).
2. Complete GitHub consent; GitHub redirects to `GET /api/auth/callback`.
3. The callback JSON includes `accessToken` — click **Authorize** in Swagger and paste `Bearer <accessToken>`.
4. Exercise Categories, Products, Entries, Exits, and Movements.

Protected inventory endpoints return **401** without a token (or with a GitHub token). Business rule violations return **400**; missing resources return **404**.

## Architecture overview

- **Domain** — inventory entities (Category, Product, Entry, Exit, Movement).
- **Application** — services and validation (stock rules, delete guards, GitHub login orchestration).
- **Infrastructure** — SQL Server via EF (reads) and Dapper (writes), GitHub OAuth client, JWT generation/validation, demo seed.
- **API** — REST controllers + global exception handling + Swagger.

See ADRs under `docs/adr/` and the Milestone 5 review in [docs/03-Architecture-Review.md](docs/03-Architecture-Review.md). AI-oriented project notes: [docs/README_AI.md](docs/README_AI.md).

## Tests

```bash
dotnet test
```

Unit tests cover Domain entities and Application business rules (xUnit, Moq, FluentAssertions).

---

## AI-Assisted Development (Cursor)

This project was developed with the assistance of AI tools as permitted by the technical assessment.

AI assistance was used as an architectural and productivity aid, including:

- Reviewing and refining the solution architecture.
- Generating and validating implementation plans before coding.
- Producing development documentation.
- Reviewing code against SOLID, Clean Architecture, and project requirements.
- Suggesting improvements and identifying potential issues during implementation.

To ensure transparency, the complete AI guidance used during development is included in the repository under the `.cursor` directory.

This documentation includes:

- Cursor Rules
- Cursor Skills
- Cursor Plans
- Cursor Chats

The implementation, validation, debugging, and final technical decisions were performed and verified throughout the development process.

---

# Author

Technical Test - Candidate: jose