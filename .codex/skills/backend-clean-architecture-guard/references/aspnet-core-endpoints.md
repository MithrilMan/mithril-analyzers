# ASP.NET Core Endpoints

## Goals

- Keep the ASP.NET host as a composition and transport boundary, not the place where business workflows live.
- Make request handling predictable and easy to review.

## Best practices

- Keep `Program.cs` focused on composition root concerns: service registration, middleware, endpoint mapping, and host wiring.
- Keep endpoint-mapping modules thin. Parse request input, call the owning service, and map the result back to HTTP.
- Keep transport-specific validation and HTTP semantics at the edge; do not leak endpoint-only DTOs deeper than necessary.
- Use dedicated request/response models when the transport shape differs from the application model.
- Make route ownership explicit: related endpoints should live together in the same mapping module.
- Do not hide workflow policy in lambda bodies or controller actions; extract that policy into services or collaborators.
- Be deliberate with defaulting and nullable binding behavior so HTTP semantics stay obvious.
- Prefer clear error mapping over silently swallowing domain or infrastructure failures.

## Heuristics

- If an endpoint file contains persistence rules, fallback strategies, or complex branching, the workflow probably belongs elsewhere.
- If several endpoints repeat the same normalization or access checks, extract the shared backend collaborator rather than duplicating transport logic.
