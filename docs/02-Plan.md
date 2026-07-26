# Inventory Management API

# Roadmap & Implementation Plan

## Summary

This document describes the implementation roadmap for the Inventory Management API.

The project is divided into incremental milestones to keep development organized and allow each stage to be validated before moving to the next one.

---

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
- Configure Entity Framework Core.
- Configure Dapper.
- Implement repositories.
- Create SQL Server database schema.
- Create initial migrations.
- Configure Docker and SQL Server.

---

# Milestone 3

## Application Layer

- Implement DTOs.
- Implement Application Services.
- Separate read operations (EF Core).
- Separate write operations (Dapper).
- Implement business rules.

---

# Milestone 4

## REST API and Security

- Implement REST endpoints.
- Configure OAuth2 authentication.
- Configure Swagger/OpenAPI.
- Implement exception handling.
- Seed demo data.

---

# Milestone 5

## Testing and Deployment

- Implement unit tests using xUnit and Moq.
- Validate application services.
- Configure Docker image.
- Configure Docker Compose.
- Validate complete application flow.