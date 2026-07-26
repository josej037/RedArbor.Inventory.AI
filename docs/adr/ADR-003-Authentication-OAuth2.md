# ADR-003: Authentication

**Status:** Accepted
**Date:** 2026-07-26

---

## Context

The API must protect its endpoints and allow authenticated users to access inventory operations.

---

## Decision

The project uses OAuth2 with JWT Bearer tokens.

Authentication is configured using the ASP.NET Core JWT Bearer middleware.

### Protected Endpoints

Authentication is required for:

- Categories
- Products
- Inventory Entries
- Inventory Exits
- Inventory Movements

The following endpoints remain public:

- Login
- Swagger

---

## Configuration

JWT settings are loaded from configuration files or environment variables.

Swagger includes Bearer authentication to simplify API testing.

---

## Consequences

### Benefits

- Stateless authentication.
- Easy integration with REST clients.
- Simple deployment in Docker environments.

### Drawbacks

- JWT signing keys must be managed securely.
- Clients must obtain and include a valid access token.


## Notes

The project uses OAuth2 concepts with JWT Bearer tokens to secure API endpoints.