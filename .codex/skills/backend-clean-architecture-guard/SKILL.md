---
name: backend-clean-architecture-guard
description: Enforce backend design guardrails for a repository. Use when modifying or reviewing backend code, especially when adding services, endpoints, extractors, clients, dependency-injection wiring, or refactors. Prevent oversized classes, preserve Clean Architecture boundaries, apply SOLID pragmatically, and ensure modified files respect any existing .editorconfig directives before finishing.
---

# Backend Clean Architecture Guard

Apply this workflow before editing backend code and re-check it before finishing.

Keep this skill focused on workflow, boundary discipline, and review checks. Stack-specific best practices live in the `references/` files and should be loaded only when the target backend surface actually uses that stack or when the current change touches it directly.

## Scope

- Treat backend endpoints, handlers, services, use cases, domain models, repositories, adapters, infrastructure clients, hosted workers, persistence code, and dependency-injection wiring as backend scope.
- Apply the skill for backend code changes, refactors, architectural reviews, and backend design proposals.
- Skip the skill for UI-only, docs-only, or ops-only changes unless they alter backend boundaries or backend workflow rules.

## Workflow

1. Read the local constraints before editing:
   - read the nearest nested `AGENTS.md` for the target backend tree, if one exists
   - treat nested `AGENTS.md` files as the repository-specific policy layer
   - inspect the local `## Stack` section and any `## Technical References` section
   - treat that AGENTS metadata as the authoritative map for which technical references from `references/` apply to the current backend surface

2. Load only the references that match the stack or the touched surface:
   - always keep the Clean Architecture rules in this skill body in view
   - if the nearest `AGENTS.md` exposes `## Technical References`, prefer that list over guessing from package names
   - read `references/dotnet-csharp-design.md` when the change touches general C# design, async flow, value objects, domain logic, or service decomposition
   - read `references/aspnet-core-endpoints.md` when host composition, endpoint mappings, request handling, or transport shaping changes
   - read `references/dependency-injection-and-options.md` when DI wiring, configuration objects, or options validation change
   - read `references/background-jobs-and-hosted-services.md` when queued execution, hosted workers, cancellation, or process orchestration changes
   - read `references/file-system-and-sqlite-persistence.md` when file-backed persistence, path policy, or local sqlite-backed runtime state changes
   - read `references/observability-serilog-opentelemetry.md` when logging, tracing, metrics, or telemetry wiring changes
   - read `references/scriban-templates.md` when template rendering or workspace prompt/materialization changes
   - read `references/xunit-testing.md` when backend tests change
   - do not bulk-read every reference by default; keep context proportional to the change

3. Enforce Clean Architecture:
   - keep dependency direction stable:
     - `Domain`: pure business types and invariants; do not reference application, infrastructure, or API concerns
     - `Application`: orchestration, use-case policy, abstractions, and options; depend on `Domain`, not on infrastructure details
     - `Infrastructure`: adapters, IO, external clients, persistence, filesystem, HTTP, and concrete implementations of application abstractions
     - `Api`: composition root, transport mapping, HTTP contracts, and request/response handling; keep business rules out of endpoint mapping and startup code
   - decide the owning layer before coding; if code needs HTTP, filesystem, process, or third-party SDK details, it does not belong in `Domain` or `Application`
   - prefer adding a new focused type in the right layer over pushing one more responsibility into an existing general-purpose service

4. Prevent large classes early:
   - treat a growing class as a design smell before it becomes a god object
   - split a class immediately when it starts to mix multiple concerns such as orchestration and domain rules, IO and policy, validation and transport mapping, parsing and persistence, or fallback strategy and external client details
   - use these heuristics:
     - one reason to change per class
     - one dependency cluster per class
     - one clear abstraction per file
   - if a new change forces a class to coordinate unrelated workflows, extract a collaborator instead of extending the class
   - prefer small orchestrators that compose focused collaborators over a single service that plans, fetches, validates, transforms, persists, and logs

5. Apply SOLID pragmatically:
   - `Single responsibility`: keep classes, handlers, services, and extractors narrowly focused
   - `Open-closed`: extend behavior through new types, strategies, or implementations instead of piling conditionals into a central class
   - `Liskov substitution`: preserve interface contracts; replacements must not weaken guarantees or require special-case callers
   - `Interface segregation`: keep interfaces small and task-specific; avoid "do everything" interfaces
   - `Dependency inversion`: depend on application-layer abstractions and wire concrete implementations through dependency injection at the edge

6. Review before editing:
   - identify the use case and the owning layer first
   - check whether the target class already has more than one reason to change
   - if the change introduces a second responsibility, plan the extraction before editing
   - if the change introduces a new cross-cutting architecture or workflow rule, update the decisions registry rooted at `docs/specs/DECISIONS.md`

7. Review before finishing:
   - re-scan each modified backend file for boundary leaks and mixed responsibilities
   - confirm that new abstractions are used only where they reduce coupling or isolate variation; do not over-engineer
   - if `.editorconfig` exists in the repository or an ancestor directory, apply its directives to every modified file before closing the task
   - if `.editorconfig` does not exist, do not invent a formatting policy; keep style consistent with neighboring files

## Review Checklist

- Does each modified type have one clear responsibility and one clear dependency cluster?
- Did endpoint mappings stay transport-focused instead of accumulating workflow policy?
- Did infrastructure details stay out of domain/application code?
- Were only the relevant stack references loaded instead of dumping unrelated guidance into the task?
- If services or orchestrators grew, were focused collaborators extracted before the file became a god object?
- If tests changed, do they verify behavior and invariants rather than internal implementation trivia?

## References

- Repository-specific backend policy: nearest nested `AGENTS.md` under the target backend tree, if present
- C# and .NET design: `references/dotnet-csharp-design.md`
- ASP.NET Core endpoint mappings: `references/aspnet-core-endpoints.md`
- Dependency injection and options: `references/dependency-injection-and-options.md`
- Hosted workers and background orchestration: `references/background-jobs-and-hosted-services.md`
- Filesystem and sqlite persistence: `references/file-system-and-sqlite-persistence.md`
- Serilog and OpenTelemetry observability: `references/observability-serilog-opentelemetry.md`
- Scriban template rendering: `references/scriban-templates.md`
- xUnit backend testing: `references/xunit-testing.md`
