# Phase 1 implementation plan
_Exported on 7/26/2026 at 23:29:30 CST from Cursor (3.13.10)_

---

**User**

Read the project documentation located in /docs together with the rules and skills under .cursor.

Before writing any code, analyze the solution and produce a detailed implementation plan for Phase 1.

Do not modify any files.

Explain:

Files to create.
Files to modify.
Dependencies.
Risks.
Architecture validation.
Whether the implementation follows the existing ADRs.

---

**Cursor**

I'll analyze the docs, Cursor rules/skills, and the current solution structure to draft a Phase 1 plan without changing any files.

Next I'll read the Cursor skills to pin down what Phase 1 scaffolding should include versus later milestones.

Checking for existing ignore/config files and confirming the intended project naming.

I have enough context: the repo is docs-only, Milestone 1 is the Phase 1 scope, and ADRs/rules/skills are already in place. Creating the detailed implementation plan.

---

**User**

Implement the plan as specified, it is attached for your reference. Do NOT edit the plan file itself.

To-do's from the plan have already been created. Do not create them again. Mark them as in_progress as you work, starting with the first one. Don't stop until you have completed all the to-dos.

---

**Cursor**

I'll implement Phase 1 per the plan, starting with `.gitignore` and `Directory.Build.props`.

Creating the solution and five projects next.

Checking what the solution template created and fixing the OpenApi vulnerability restore issue.

Creating a classic `.sln`, removing the vulnerable OpenApi package, and wiring project references.

Adding the architecture smoke test and cleaning up the test project.

Building and testing the solution next. Leaving the empty Controllers folder as-is.

Removing the empty Controllers folder left by the template.

Phase 1 foundation is in place.

**Created**
- `Inventory.sln`, `.gitignore`, `Directory.Build.props` (`net10.0`, nullable, warnings as errors)
- `src/Inventory.Domain`, `Application`, `Infrastructure`, `Api`
- `tests/Inventory.Tests` with `ArchitectureSmokeTests`
- Minimal runnable `Program.cs` (no auth, OpenAPI, or sample controllers)

**Dependencies**
- Project refs match ADR-002
- No EF Core, Dapper, JWT, or Docker
- Tests: xUnit only (per plan; Moq/FluentAssertions deferred)

**Validation**
- `dotnet build` — 0 warnings, 0 errors  
- `dotnet test` — 2 passed

