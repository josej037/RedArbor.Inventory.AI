## Purpose

Secure the API using OAuth2 with JWT Bearer tokens.

---

## Guidelines
- Protect endpoints with Authorize.
- Allow login anonymously.
- Generate signed JWT tokens.
- Include user identity claims.

---

## Rules
- Never expose passwords.
- Never hardcode secrets.
- Read JWT configuration from appsettings or environment variables.