# Dependency Injection And Options

## Goals

- Keep composition explicit and lifecycle-safe.
- Make configuration and dependencies easy to trace.

## Best practices

- Register dependencies close to the composition root and keep the registration shape aligned with the architecture.
- Prefer constructor injection for required collaborators.
- Keep interfaces small and use them to isolate variation, not to wrap every concrete class by default.
- When options represent a durable configuration boundary, model them as dedicated option types and validate them near startup or before use.
- Keep defaults explicit and near the option definition or the host configuration source.
- Avoid service locators and runtime `GetService` calls in application logic.
- Be honest about lifetimes: singleton services should not capture scoped or transient state accidentally.
- If a dependency exists only for one narrow responsibility, consider extracting that responsibility into its own collaborator instead of widening an existing service constructor further.

## Heuristics

- A constructor with many unrelated dependencies usually points to a missing decomposition.
- If registration code is full of conditional branches, consider whether the variation should be modeled as strategy types or option-driven collaborators.
