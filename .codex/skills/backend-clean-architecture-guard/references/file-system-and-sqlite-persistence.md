# Filesystem And Sqlite Persistence

## Goals

- Keep file-backed persistence explicit, safe, and reviewable.
- Preserve invariants around paths, serialization, and update sequencing.

## Best practices

- Treat path resolution and path policy as first-class responsibilities; do not scatter ad-hoc path building across unrelated services.
- Keep serialization boundaries explicit and version-tolerant where persisted data survives restarts.
- Use dedicated stores or repository-like collaborators as the persistence owner for a given file family.
- Keep directory creation, file IO, and normalization inside the persistence layer or infrastructure collaborators.
- Prefer atomic or replace-style updates when partially written files would corrupt the runtime contract.
- When sqlite is used as a local structured store, keep schema ownership and access patterns isolated from higher-level orchestration.
- Validate external file inputs and archive extraction paths defensively.

## Heuristics

- If business policy depends on raw file layout knowledge everywhere, persistence ownership is leaking.
- If a service both decides what should be stored and also knows too much about the file format or path layout, consider splitting policy from store implementation.
