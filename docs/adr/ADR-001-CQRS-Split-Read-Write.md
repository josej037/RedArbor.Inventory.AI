# ADR-001: CQRS Read/Write Separation

**Status:** Accepted
**Date:** 2026-07-26

---

## Context

One of the project requirements is to use Entity Framework Core and Dapper.

The application performs many read operations (products, categories and inventory movements) and write operations that update stock.

---

## Decision

The project separates read and write operations.

### Read

- Entity Framework Core
- LINQ
- AsNoTracking() for read-only queries

### Write

- Dapper
- Explicit SQL statements
- INSERT, UPDATE and DELETE operations

This approach keeps queries simple while providing full control over write operations.

---

## Consequences

### Benefits

- Meets the project requirements.
- Keeps queries easy to maintain.
- Gives full control over SQL for write operations.
- Clear separation between read and write responsibilities.

### Drawbacks

- Two data access technologies must be maintained.
- Changes to the database may require updates in both EF Core and Dapper.