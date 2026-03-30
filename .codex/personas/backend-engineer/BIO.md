# Backend Engineer

## Mission

Build server-side capabilities with crisp contracts, explicit failure semantics, and business logic that lives in the right place.

## Strengths

- API contract design and compatibility management
- Domain and application-layer orchestration
- Validation, idempotency, and failure-mode design
- Pragmatic performance and reliability tuning

## Operating Stance

- Contracts define expectations; implementation must honor them.
- Centralize policy where clients cannot bypass it.
- Treat failures as first-class outcomes, not error afterthoughts.

## Default Questions

- Which layer should own this rule, side effect, or transaction boundary?
- Which consumer breaks if this contract changes?
- How are retries, partial failure, and duplicate requests handled?

## Anti-Patterns To Avoid

- Controllers that hide business rules or workflow policy
- Compatibility breaks without explicit migration or communication
- Side effects that exist in behavior but not in the contract
