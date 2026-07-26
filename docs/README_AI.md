# AI Development Guide

## Overview

This repository is intended to be developed with AI assistance (Cursor).

The purpose of this document is to provide enough context so that every implementation follows the same architecture, coding standards and design decisions throughout the project.

Before generating code, review the project documentation located in the folder.

---

## Project

This project implements an Inventory Management REST API using Microsoft technologies.

Main technologies:

- .NET 10
- ASP.NET Core Web API
- SQL Server
- Entity Framework Core
- Dapper
- JWT Bearer Authentication
- Docker
- xUnit

The objective is to build a clean, maintainable solution for Inventory Management.

---

## Documentation

All project documentation is located under the `docs` directory.

```text
docs
│
├── README_AI.md
├── Understanding.md
├── Plan.md
├── adr/
```

---

## Recommended Reading Order

When working on a feature, review the documentation in the following order:

1. Understanding.md
2. Plan.md
3. Related ADR

---

## Development Workflow

For each feature:

1. Understand the requirement.
2. Review the architecture.
3. Check if an ADR already exists.
4. Follow the implementation plan.
5. Generate the code.
6. Validate the implementation.
7. Update tests if necessary.

If any requirement is unclear, stop and request clarification before continuing.

---

## General Guidelines

- Follow Clean Architecture.
- Apply SOLID principles.
- Keep Controllers focused on HTTP concerns.
- Business rules belong to the Application layer.
- Infrastructure should only contain external dependencies.
- Use dependency injection.
- Prefer asynchronous APIs.
- Avoid duplicated code.
- Keep implementations simple.
- Write readable code before clever code.

---

## Project Documentation

### ADR

Architectural decisions are documented in:

```text
docs/adr
```

Review the corresponding ADR before changing the architecture.

---

### Skills

Technical implementation guidelines are stored in:

```text
.cursor/skills
```

Each skill contains recommendations, best practices and examples for a specific technology or design pattern.

---

## Definition of Done

Before considering a task complete, verify:

- The solution compiles successfully.
- Existing tests continue passing.
- New functionality is covered by tests when applicable.
- The architecture has not been compromised.
- No duplicated logic has been introduced.
- Documentation is updated if required.

---

## Notes

Prefer extending the existing architecture instead of introducing new patterns or dependencies.

If multiple implementation options exist, choose the simplest one that satisfies the project requirements.