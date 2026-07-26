## Purpose

Implement write operations.

---

## Guidelines

- Use parameterized SQL.
- Keep SQL readable.
- Return generated IDs when needed.
- Use transactions when multiple writes are required.

---

## Avoid

- SQL string concatenation.
- Dynamic SQL when unnecessary.
- Business logic inside SQL.