# React Router

Official sources consulted:

- https://reactrouter.com/

## Goals

- Keep routes as explicit composition boundaries.
- Use the URL for durable, shareable state; keep ephemeral UI state out of the URL.

## Best practices

- `pages/` should map cleanly to route-owned screens or route fragments.
- Read route params close to the page/model boundary, then pass normalized values downward.
- Keep navigation side effects in page or feature model hooks, not scattered across deep UI components.
- Only place state in the URL when it should survive reload/share/back-forward semantics.
- Do not overload the URL with transient UI-only concerns such as temporary panel toggles unless that state is intentionally shareable.
- Prefer declarative links for navigation UI and imperative navigation only for workflow transitions.
- Normalize optional params and search params once before deriving queries or local state from them.
- When route changes should reset workspace-local state, make that reset explicit in the owning page model.

## Heuristics

- If a child widget needs to know too much about route structure, pull that logic back up into the page model.
- If a URL parameter is only ever used by one derived query or selector, parse it once and pass the clean value down.
