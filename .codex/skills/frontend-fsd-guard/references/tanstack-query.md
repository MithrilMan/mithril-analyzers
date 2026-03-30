# TanStack Query

Official sources consulted:

- https://tanstack.com/query/latest/docs/framework/react/overview
- https://tanstack.com/query/latest/docs/framework/react/guides/query-keys
- https://tanstack.com/query/latest/docs/framework/react/guides/updates-from-mutation-responses
- https://tanstack.com/query/latest/docs/framework/react/guides/optimistic-updates

## Goals

- Keep server state in TanStack Query, not in ad-hoc local stores.
- Make cache ownership, invalidation, and patching predictable.

## Best practices

- Query keys must be stable, serializable, and shaped around the real resource boundary.
- TanStack Query owns remote snapshots, pending/error states, and mutation lifecycle for backend-backed data.
- Do not copy query results into Zustand or local state unless the UI needs a true editable draft or offline-only staging area.
- Keep query functions thin and side-effect free; normalization belongs in `select`, pure helpers, or post-parse mappers.
- Prefer targeted `setQueryData` patches when realtime events or mutation responses can update one known cache entry safely.
- Follow uncertain or broad changes with invalidation so the cache can reconcile from the server.
- Keep optimistic updates rare and explicit; if rollback logic is complex, a standard invalidate-on-success path is usually safer.
- Co-locate mutations with the page/feature/entity model that owns the user action, not inside deeply presentational components.
- Derived UI projections should stay derived; avoid introducing duplicate cache-shaped state elsewhere.

## Heuristics

- If a store contains data that also exists in Query and both need reconciliation, ownership is probably blurred.
- If a query key is hard to explain in one sentence, its boundary may be wrong.
- If a mutation updates many caches manually, consider whether the server/read model boundary should be simpler or whether invalidation is the safer tradeoff.
