## Purpose

Separate read and write operations.

---

## Queries

- Use Entity Framework Core.
- Return DTOs.
- Use LINQ.
- Prefer AsNoTracking().

---

## Commands

- Use Dapper.
- Execute explicit SQL.
- Handle INSERT, UPDATE and DELETE.

---

## Rules

- Queries must not modify data.
- Commands must not return complex objects.
- Keep each operation focused on a single responsibility.