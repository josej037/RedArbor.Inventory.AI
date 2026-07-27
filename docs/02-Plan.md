# Inventory Management API

# Roadmap & Implementation Plan

## Summary

This document describes the implementation roadmap for the Inventory Management API.

The project is divided into incremental milestones to keep development organized and allow each stage to be validated before moving to the next one.

---

# Note

Every milestone must finish with:

- Build validation.
- Existing tests passing.
- Documentation update if required.
- Code review before starting the next milestone.

# Milestone 1

## Project Foundation

- Create the solution and layered architecture.
- Configure project dependencies.
- Configure Cursor rules, skills and ADRs.
- Define coding standards and development guidelines.

---

# Milestone 2

## Domain and Infrastructure

- Define domain entities.
- Create SQL Server database schema.
- Configure Entity Framework Core.
- Configure Dapper.
- Implement repositories.
- Create initial migrations.
- Configure Docker and SQL Server.

---

# Milestone 3

## Application Layer

- Implement Application Services.
- Implement DTOs.
- Implement business rules.
- Separate read operations (EF Core).
- Separate write operations (Dapper).

---

# Milestone 4

## REST API and Security

- Implement REST endpoints.
- Implement exception handling.
- Configure OAuth2 authentication.
- Configure Swagger/OpenAPI.
- Seed demo data.

---

# Milestone 5

## Testing and Deployment

- Implement unit tests using xUnit and Moq.
- Validate application services.
- Validate Application Services.
- Configure Docker image.
- Configure Docker Compose.
- Validate complete application flow.