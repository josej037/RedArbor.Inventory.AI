## Purpose

Implement features following the project's layered architecture.

## Layers

### Domain

Contains:

- Entities
- Value Objects
- Enums
- Domain Exceptions

Do not reference Infrastructure or API.

---

### Application

Contains:

- Application Services
- Commands
- Queries
- DTOs
- Interfaces
- Business Rules

Do not access the database directly.

---

### Infrastructure

Contains:

- EF Core
- Dapper
- Repository implementations
- Authentication
- External services

Do not implement business rules.

---

### API

Contains:

- Controllers
- Middleware
- Dependency Injection
- Swagger

Controllers should only handle HTTP requests and responses.

---

## Rules

- Respect dependency direction.
- Keep each layer focused on its responsibility.
- Avoid circular references.