## Purpose

Run the application locally using containers.

---

## Guidelines
- Use Docker Compose.
- Keep configuration in environment variables.
- SQL Server runs in a separate container.
- API connects using the service name.

---

## Rules
- Do not hardcode connection strings.
- Keep containers independent.
- Persist database data using volumes.