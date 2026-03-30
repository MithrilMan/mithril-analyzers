# Zod Contracts

Official sources consulted:

- https://zod.dev/

## Goals

- Keep runtime validation and TypeScript types aligned with the real backend contract.
- Make contract drift visible at the boundary instead of leaking invalid shapes deeper into the UI.

## Best practices

- Parse external data at the boundary where it enters the frontend.
- Prefer generated or shared contract sources over hand-maintained duplicate schemas when the backend already defines the shape.
- Keep the schema as the source of truth for optionality, nullability, discriminated unions, and enum-like values.
- Normalize data after parse, not before; parsing should validate the transport shape first.
- Keep UI-specific projection types separate from transport contracts.
- When a backend payload changes, update the schema and the consuming projections in the same phase.
- Do not bypass parsing for "trusted" endpoints if the rest of the stack assumes validated shapes.

## Heuristics

- If UI code is full of optional chaining against fields that should be required, check the schema and normalization path.
- If several components each reinterpret the same backend field, introduce one parsed projection helper.
