# Milestone 3 implementation plan
_Exported on 7/26/2026 at 23:31:01 CST from Cursor (3.13.10)_

---

**User**

Generate the implementation plan for Milestone 3 defined in docs/02-Plan.md .

Scope of this milestone only:

- Implement Application Services.
- Implement DTOs.
- Implement business rules.
- Separate read operations (EF Core).
- Separate write operations (Dapper).

Do not implement the code yet.

The plan must include:

- Scope
- Current state vs expected state
- Files to create
- Files to modify
- Project dependencies
- NuGet packages
- Implementation order
- Risks
- Validation steps
- Definition of Done

Follow all accepted ADRs, Cursor Rules and Skills.

Do not include anything from Milestone 4 or 5.

---

**Cursor**

I'll research Milestone 3 in the plan docs and the current Application/CQRS structure so the implementation plan matches the ADRs and skills.

I'll pull domain entities, deferred M2 business rules, and Application-layer conventions so the plan stays concrete.

I have enough context: M2 left Application as persistence abstractions only; M3 owns services, DTOs, and entry/exit→stock/movement orchestration. Creating the plan.

---

**User**

Review the Milestone 3 implementation plan and simplify it before implementation.

The goal of this project is to build a clean, maintainable solution that follows the accepted ADRs, but without introducing unnecessary abstractions or enterprise-level patterns that do not add value for this technical assessment.

Keep the current architecture (Clean Architecture + CQRS + EF Core for reads + Dapper for writes), but make the following adjustments:

1. Remove IUnitOfWork and any custom UnitOfWork implementation.
   - Do not introduce a transaction abstraction.
   - Dapper repositories can manage transactions directly using IDbConnection and IDbTransaction when required.
   - Keep transaction handling simple and localized.

2. Remove InventoryMovementService.
   - InventoryMovement is not an independent business process.
   - It is generated automatically by InventoryEntryService and InventoryExitService.
   - Only repository query methods are needed for reading movement history.

3. Simplify the exception hierarchy.
   - Remove unnecessary inheritance levels.
   - Keep only:
     - BusinessException
     - NotFoundException
   - Use BusinessException for validation and business rule violations (insufficient stock, delete restrictions, etc.).

4. Keep only four Application Services:
   - CategoryService
   - ProductService
   - InventoryEntryService
   - InventoryExitService

5. Make it explicit that:
   - Application Services orchestrate business rules only.
   - They must never contain SQL.
   - They must never depend on DbContext.
   - They only use repository interfaces defined in the Application layer.

6. Keep DTOs exactly as planned.
   Do not introduce AutoMapper or additional mapping libraries.
   Perform manual mapping inside the services.

7. Keep CQRS simple.
   - Query methods use EF Core repositories.
   - Command methods use Dapper repositories.
   - Do not introduce MediatR, pipeline behaviors, or command/query handlers.

8. Simplify the unit testing plan.
   - Focus only on:
     - CategoryService
     - ProductService
     - InventoryEntryService
     - InventoryExitService
   - Remove InventoryMovementService tests.

9. Update the Definition of Done accordingly.

The final Milestone 3 plan should remain fully aligned with the existing ADRs while avoiding overengineering. The objective is readability, maintainability, and simplicity rather than introducing additional architectural layers.

10. Clarify layer responsibilities.

- Domain:
  Entities and domain validations only.

- Application:
  DTOs, business rules, orchestration, repository interfaces and services.

- Infrastructure:
  EF Core, Dapper, repository implementations and database access only.

- API:
  Controllers, authentication, middleware and dependency injection.

Business rules must never be implemented inside repositories or controllers.
Repositories must only perform data access.

---

**Cursor**

I'll update the Milestone 3 plan to drop UnitOfWork and MovementService, simplify exceptions to BusinessException/NotFoundException, and keep only the four Application services.

Rewriting the plan to match the simplified architecture.

The Milestone 3 plan is updated for a simpler assessment-friendly design. Main changes:

1. **Removed `IUnitOfWork`** — multi-step atomicity stays in Infrastructure via local `IDbConnection` / `IDbTransaction` when needed.
2. **Removed `InventoryMovementService`** — movements are created only by Entry/Exit services; history uses existing repository Get methods.
3. **Exceptions** — only `BusinessException` and `NotFoundException`.
4. **Four services only** — Category, Product, InventoryEntry, InventoryExit.
5. **Layer rules made explicit** — Domain / Application / Infrastructure / API; no business rules in repos or controllers; services never use SQL or `DbContext`.
6. **DTOs unchanged** — manual mapping, no AutoMapper.
7. **CQRS stays simple** — Query → EF repos; Command → Dapper repos; no MediatR.
8. **Tests** — only the four services (no movement service tests).
9. **Definition of Done** — aligned with the above.
10. **Layer responsibilities** — documented in the plan.

Ready to implement when you approve.

