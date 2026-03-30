# Zustand + Immer

Official sources consulted:

- https://github.com/pmndrs/zustand
- https://github.com/pmndrs/zustand/blob/main/docs/learn/guides/flux-inspired-practice.md
- https://github.com/pmndrs/zustand/blob/main/docs/learn/guides/advanced-typescript.md
- https://github.com/pmndrs/zustand/blob/main/docs/learn/guides/beginner-typescript.md
- https://immerjs.github.io/immer/

## Goals

- Keep local shared UI state readable, bounded, and safe to evolve.
- Use Immer for clear mutations without turning the store into a dumping ground.

## Best practices

- Prefer local React state first. Reach for Zustand only when UI-only state is shared across multiple components or widgets.
- Use one store per bounded workflow/workspace area, then compose it from thematic slices as the surface grows.
- Keep invariants that must change together inside the same owning slice/store.
- Do not move server state into Zustand just to simplify prop flow.
- Expose explicit actions with names that describe intent, not implementation details.
- Keep selectors narrow. Subscribe to only the fields/actions a consumer needs.
- When returning aggregated objects from a selector, use `useShallow` unless another equality strategy is already justified.
- Keep reset actions deterministic and complete; they should restore the owned UI boundary to a known baseline.
- Avoid hidden coupling through broad "set everything" actions unless the store is an intentional form draft boundary.
- If one slice starts to own unrelated concerns, split the slice before creating another top-level store.

## Heuristics

- A store file that mixes dialogs, filters, drafts, routing, and domain data likely needs decomposition.
- Many tiny global stores for one workflow usually mean the real bounded context has been split too aggressively.
- If a component needs only one or two booleans, subscribe to those booleans instead of a broad object.
