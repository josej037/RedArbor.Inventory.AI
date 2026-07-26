# ADR-002: Clean Architecture

**Status:** Accepted
**Date:** 2026-07-26

---

## Context

The project needs a clear structure to separate business logic, data access and the API layer.

The architecture should be easy to maintain and test.

---

## Decision

The solution follows a Clean Architecture approach using four projects.

```text
Inventory.Api
Inventory.Application
Inventory.Domain
Inventory.Infrastructure
Inventory.Tests
```

### Responsibilities

**Inventory.Domain**
- Entities
- Enums
- Value Objects
- Exceptions

**Inventory.Application**
- Application Services
- DTOs
- Interfaces
- Business Rules

**Inventory.Infrastructure**
- Entity Framework Core
- Dapper
- Authentication
- Repository implementations

**Inventory.Api**
- Controllers
- Middleware
- Swagger
- Dependency Injection

---

## Dependency Rules

- Domain has no dependencies.
- Application depends on Domain.
- Infrastructure depends on Application and Domain.
- Api depends on Application and Infrastructure.

---

## Consequences

### Benefits

- Clear separation of responsibilities.
- Easier unit testing.
- Better maintainability.
- Business logic remains independent from infrastructure.

### Drawbacks

- More projects to maintain.
- DTO mapping between layers.