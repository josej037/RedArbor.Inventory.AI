# Inventory Management API

# Project Understanding

## Summary

This project implements a RESTful Inventory Management API built with Microsoft technologies.

The application allows users to manage:

- Product Categories
- Products
- Inventory Entries (Inbound Movements)
- Inventory Exits (Outbound Movements)
- Inventory Movement History

---

# Objectives

The solution should:

- Provide RESTful endpoints.
- Manage product stock automatically.
- Record every inventory movement.
- Protect endpoints using OAuth2 authentication.
- Be fully containerized with Docker.
- Include unit tests for the application layer.

---

# Technology Stack

## Language & Framework

- C#
- .NET 10
- ASP.NET Core Web API

## Database

- SQL Server 2022

## Data Access

### Read Operations

- Entity Framework Core

### Write Operations

- Dapper

## Authentication

- Authentication based on OAuth2.

## Documentation

- Swagger / OpenAPI

## Testing

- xUnit
- Moq

## Containers

- Docker
- Docker Compose

---

# Architecture

The solution follows a layered architecture inspired by Clean Architecture.

Projects are organized as:

- API
- Application
- Domain
- Infrastructure
- Tests

Read operations are implemented using Entity Framework Core.

Write operations are implemented using Dapper.

Business rules are encapsulated within the Application layer, while the Domain layer remains independent from infrastructure concerns.

---

# Design Decisions

The project intentionally keeps the architecture simple while demonstrating professional development practices.

Key decisions include:

- Entity Framework Core for queries.
- Dapper for commands.
- OAuth2 for authentication.
- SQL Server as the relational database.
- Docker for local execution.
- Unit testing with xUnit and Moq.

The goal is to balance maintainability, readability, and simplicity without introducing unnecessary complexity.