## Purpose

Implement read operations.

---

## Guidelines

- Use LINQ.
- Use AsNoTracking() for read-only queries.
- Use Include() only when necessary.
- Project directly to DTOs.
- Keep queries simple.

---

## Avoid

- Business logic inside queries.
- Loading unnecessary data.
- Returning entities to the API.