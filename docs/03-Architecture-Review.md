# Architecture and Code Quality Review

**Date:** 2026-07-26  
**Scope:** Milestone 5 final review against ADRs, Cursor rules, and Docker skill  
**Nature:** Analysis and recommendations only — no redesign required for Milestone 5 completion

---

## Summary

The solution aligns with the accepted ADRs and workspace rules: Clean Architecture layers, EF Core reads / Dapper writes, thin controllers, JWT Bearer protection, and env-driven Docker Compose configuration. Application and Domain unit coverage is adequate for the current business rules after Milestone 5 gap-fill. Remaining items below are advisory improvements for later milestones.

---

## ADR-001 — CQRS (EF reads / Dapper writes)

### Findings

- Read paths use EF Core repositories with `AsNoTracking()` for list/get queries.
- Write paths (INSERT/UPDATE/DELETE and stock-changing transactions) use Dapper with explicit SQL.
- Stock-affecting entry/exit flows keep product updates and movement history in coordinated repository methods.

### Assessment

Compliant with ADR-001. The dual data-access approach is intentional and documented.

### Recommendations

1. When schema changes land, keep EF configurations and Dapper SQL column lists updated together (already noted as an ADR drawback).
2. Consider documenting the transactional write methods (`CreateWithStockAndMovementAsync`, `DeleteWithStockAsync`) in a short Infrastructure note so future contributors preserve atomicity.

---

## ADR-002 — Layer boundaries and dependency direction

### Findings

| Project | Role | Observed |
|---------|------|----------|
| `Inventory.Domain` | Entities, enums | No outward project dependencies |
| `Inventory.Application` | Services, DTOs, repository abstractions, business rules | Depends on Domain only |
| `Inventory.Infrastructure` | EF, Dapper, JWT, repositories, seed | Depends on Application + Domain |
| `Inventory.Api` | Controllers, Swagger, exception handling, host bootstrap | Depends on Application + Infrastructure |
| `Inventory.Tests` | Domain + Application unit tests | Depends on Application + Domain |

- Controllers call Application services only; no `DbContext` injection in controllers.
- Business validations (empty names, negative stock/price, delete guards, insufficient stock) live in Application services.

### Assessment

Compliant with ADR-002 and Cursor Clean Architecture rules.

### Recommendations

1. Keep mapping logic in Application (or dedicated mappers later) — avoid pushing domain entities through API contracts.
2. If the API surface grows, prefer additional Application services over thickening controllers.

---

## ADR-003 — JWT Bearer authentication

### Findings

- Inventory endpoints require `[Authorize]`; `POST /api/auth/login` and Swagger remain anonymous.
- JWT Issuer/Audience/Key are loaded from configuration; Infrastructure fails fast when missing or when the key is shorter than 32 characters.
- Swagger is configured for Bearer tokens to support manual E2E testing.

### Assessment

Compliant with ADR-003 for this project's OAuth2-with-JWT-Bearer approach (configured demo credentials, not an external IdP).

### Recommendations

1. Replace demo username/password with a real identity provider before any non-demo deployment.
2. Store production JWT signing keys in a secret store; never commit real keys (`.env` is gitignored; `.env.example` documents placeholders only).
3. Consider rotating demo credentials in shared environments and shortening token lifetime for higher-risk deployments.

---

## Cursor rules compliance

| Rule | Status |
|------|--------|
| Async methods use `Async` suffix and accept `CancellationToken` | Met in Application/Infrastructure surfaces reviewed |
| No `DbContext` in controllers | Met |
| Business rules in Application | Met |
| CQRS split (EF reads / Dapper writes) | Met |
| Connection strings / secrets via env or `appsettings` | Met |
| SOLID / thin controllers | Met for current scope |

### Recommendations

1. Continue treating warnings as errors (`Directory.Build.props`) for all new code.
2. Prefer primary constructors and file-scoped namespaces as already used across the solution.

---

## Test coverage (after Milestone 5 gap-fill)

### Findings

- Suite: **57** Domain/Application unit tests (xUnit, Moq, FluentAssertions), all green.
- Gap-fill covered:
  - Product update validations (empty name, negative stock, negative unit price, missing category, happy path)
  - Product delete when missing
  - Category empty-name update
  - Inventory entry delete when stock is insufficient to reverse
- Domain entity construction/validation and Auth login paths remain covered.

### Assessment

Adequate for current business rules. Thin read-mapping tests were correctly skipped per Milestone 5 scope.

### Recommendations

1. Add API integration tests later if regression risk around auth middleware or HTTP status mapping becomes a concern (out of M5 scope).
2. Keep one-behavior-per-test naming when extending Application coverage.

---

## Docker skill compliance

### Findings

- Multi-stage `Dockerfile` builds/publishes `Inventory.Api` on .NET 10 (`sdk` → `aspnet`), exposes port `8080`.
- Compose runs `api` + `sqlserver`; API connects by service name `sqlserver`.
- SQL data persists via the existing `sqlserver_data` volume.
- Configuration is env-driven (`ConnectionStrings__InventoryDb`, `Jwt__*`, `Auth__*`); secrets are not hardcoded in the image.
- API waits on SQL healthcheck (`depends_on: condition: service_healthy`).
- Pending EF migrations apply at API startup before demo seed (host bootstrap only; migration files unchanged).

### Assessment

Compliant with the Docker skill for local Compose operation.

### Recommendations

1. If first-boot races appear on slower hosts, add a short retry around `MigrateAsync` only — avoid new frameworks.
2. Persist ASP.NET Data Protection keys (container warning today) if cookie/DP features are added later; JWT-only flows are unaffected for this API.
3. Keep mssql-tools18 healthcheck as-is unless the base image path changes.

---

## Non-critical observations (do not implement in M5)

- Auth is configuration-backed demo credentials, not a full user store — acceptable for the assignment, not for production.
- Seed skips when any category/product exists; intentional for idempotent boot, but means partial DBs will not backfill missing demo rows.
- Dual EF/Dapper maintenance cost remains the primary long-term operational trade-off (ADR-001).

---

## Conclusion

Milestone 5 validation targets are met without architectural redesign. The codebase respects ADRs and Cursor rules; recommendations above are optional follow-ups and should not block delivery.
