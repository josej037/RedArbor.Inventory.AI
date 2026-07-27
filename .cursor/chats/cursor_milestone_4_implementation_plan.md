# Milestone 4 implementation plan
_Exported on 7/26/2026 at 23:31:46 CST from Cursor (3.13.10)_

---

**User**

Generate the implementation plan for Milestone 4 defined in docs/02-Plan.md. 

Scope of this milestone only:

- Implement REST API endpoints.
- Configure global exception handling.
- Configure OAuth2 authentication using JWT Bearer.
- Configure Swagger/OpenAPI.
- Seed demo data.

Do not implement any code yet.

The plan must include:

- Scope
- Current state vs expected state
- Files to create
- Files to modify
- Project dependencies
- Required NuGet packages
- Implementation order
- Risks
- Validation steps
- Definition of Done

The plan must follow all accepted ADRs, Cursor Rules and Skills.

Keep the solution simple and avoid unnecessary abstractions or enterprise patterns that are not required for this project.

Respect the existing architecture:

- Controllers must only handle HTTP concerns.
- Business rules must remain in the Application layer.
- Infrastructure must contain authentication, persistence and external integrations only.
- Controllers must never access repositories directly.
- Controllers must communicate only with Application Services.
- Swagger configuration must remain isolated from business logic.
- Global exception handling must be implemented through middleware or exception handler, not inside controllers.
- OAuth2 authentication must follow the project ADR without introducing an external Identity Provider.

The authentication section must clearly describe:

- Which OAuth2 flow will be implemented.
- How JWT Bearer integrates with OAuth2.
- Which endpoints will be public.
- Which endpoints will require authentication.
- How Swagger will authenticate requests.

Do not include implementation details from Milestone 5.

Return only the implementation plan.

---

**Cursor**

I'll gather Milestone 4 requirements from the plan docs, ADRs, and skills, then map them against the current API/Application state.

I have enough context from the ADRs, skills, and current Application/API state to draft a concrete Milestone 4 plan.

---

**User**

Review the Milestone 4 implementation plan and simplify it.

The goal is to keep the architecture appropriate for a small technical assessment, avoiding unnecessary abstractions.

Make the following changes:

1. Authentication
- Remove IDemoUserValidator.
- Remove DemoUserValidator.
- Do not introduce additional authentication abstractions unless strictly necessary.
- Keep only:
  - AuthController
  - IAuthService
  - AuthService
  - JwtTokenGenerator
- AuthService should validate the configured username/password directly from configuration and then generate the JWT.

2. Inventory Movements
- Remove IInventoryMovementService.
- Remove InventoryMovementService.
- Inventory movements are generated only as a side effect of InventoryEntry and InventoryExit.
- Reading movements should use the existing repository directly through the existing application flow without creating another service layer.

3. OAuth2
- Remove the long explanation about:
  - Authorization Server
  - Resource Server
  - Password Grant
  - OAuth2 Flow
- Replace it with a simple implementation note:
  - Configure JWT Bearer Authentication.
  - Expose POST /api/auth/login.
  - Return a JWT token after validating configured credentials.
  - Protect the remaining endpoints using [Authorize].
  - Configure Swagger with Bearer authentication.

4. Demo Data
- Seed only Categories and Products.
- Do not seed Inventory Entries, Inventory Exits or Inventory Movements.
- Those records must be created naturally through the API.

5. Exception Handling
- Keep only:
  - NotFoundException -> HTTP 404
  - BusinessException -> HTTP 400
  - Exception -> HTTP 500

6. Keep the rest of the plan unchanged.

Do not add new patterns, services or abstractions.
Do not introduce ASP.NET Identity, IdentityServer, OpenIddict, refresh tokens, roles, policies, MediatR, AutoMapper, UnitOfWork or any enterprise authentication infrastructure.

The objective is to keep the solution simple, readable and aligned with the project requirements while following the accepted ADRs.

---

**Cursor**

Updating the plan to remove the extra auth/movement abstractions and simplify the OAuth, seed, and exception sections.

The Milestone 4 plan is simplified. Summary of changes:

1. **Auth** — Dropped `IDemoUserValidator` / `DemoUserValidator`. Kept `AuthController`, `IAuthService`, `AuthService`, `JwtTokenGenerator`. `AuthService` reads credentials from configuration. Kept only `IJwtTokenGenerator` so Application does not reference Infrastructure.

2. **Movements** — Removed `IInventoryMovementService`, `InventoryMovementService`, `InventoryMovementsController`, and movement endpoints. Movements stay side effects of Entry/Exit only.

3. **OAuth2** — Replaced the long flow/server explanation with the short implementation note you specified.

4. **Seed** — Categories and Products only; no entries/exits/movements.

5. **Exceptions** — Only `NotFoundException` → 404, `BusinessException` → 400, `Exception` → 500.

Everything else (endpoint map for the four resources, packages, middleware order, validation, DoD) stays aligned with the simplified approach.

---

**User**

Review the Milestone 4 implementation plan and apply the following refinements without changing its overall scope.

1. Authentication Configuration
- Do not inject IConfiguration directly into AuthService.
- Keep Clean Architecture boundaries.
- Introduce a minimal abstraction (for example, IAuthConfiguration) in the Application layer.
- Infrastructure must implement this abstraction using IOptions<AuthOptions>.
- AuthService should depend only on abstractions.

2. Inventory Movements Read API
- Add a read-only InventoryMovementsController.
- Expose only:
  - GET /api/inventory/movements
  - GET /api/inventory/movements/{id}
- Do NOT add POST, PUT or DELETE endpoints.
- Movements must continue being generated only as a side effect of Inventory Entry and Inventory Exit operations.
- The controller must only call an Application service and must not access repositories directly.

3. Demo Authentication
- Update the Demo Seed section to explicitly document the demo credentials used for Swagger testing.
- Do NOT create a Users table.
- Credentials must come from configuration (appsettings.Development.json or environment variables).

4. Validation Steps
- Add a validation step to verify that Product.Stock is updated correctly after every Inventory Entry and Inventory Exit operation.
- This validation should explicitly confirm that stock increases after entries and decreases after exits.

5. Definition of Done
- Add the following completion criterion:
  - Swagger XML documentation is enabled and all REST endpoints are documented.

Keep everything else unchanged.

Do not introduce:
- ASP.NET Identity
- IdentityServer
- OpenIddict
- Refresh Tokens
- Roles
- Policies
- MediatR
- AutoMapper
- FluentValidation
- UnitOfWork
- Any additional enterprise abstractions

The final plan must remain aligned with ADR-001, ADR-002, ADR-003, the Cursor Rules, and the existing project scope.

---

**Cursor**

Updating the plan with auth configuration abstraction, read-only movements API, demo credentials, stock validation, and Swagger XML docs.

The Milestone 4 plan is updated with these refinements:

1. **Auth config** — `IAuthConfiguration` in Application; Infrastructure implements it with `IOptions<AuthOptions>`. `AuthService` depends only on `IAuthConfiguration` + `IJwtTokenGenerator` (no `IConfiguration`).

2. **Movements read API** — Read-only `InventoryMovementsController` + thin `IInventoryMovementService` for `GET /api/inventory/movements` and `GET /api/inventory/movements/{id}` only. Writes stay Entry/Exit side effects; `GetAllAsync` added on the movement repository.

3. **Demo auth** — Credentials documented as Development config (e.g. `demo` / `Demo123!`); no Users table.

4. **Validation** — Explicit stock checks: increases after entry, decreases after exit.

5. **DoD** — Swagger XML documentation enabled for all REST endpoints.

Everything else is unchanged; no enterprise auth/patterns added.

