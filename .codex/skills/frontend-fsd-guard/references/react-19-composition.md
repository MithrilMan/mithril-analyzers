# React 19 Composition

Official sources consulted:

- https://react.dev/
- https://react.dev/learn
- https://react.dev/reference/react/startTransition
- https://react.dev/reference/react/useDeferredValue
- https://react.dev/reference/react/useEffect
- https://react.dev/learn/separating-events-from-effects

## Goals

- Keep React code explicit, resilient, and easy to evolve.
- Prefer composition and derivation over duplicated state and effect-heavy orchestration.
- Use React 19 concurrency tools deliberately, not decoratively.

## Best practices

- Keep route/page hooks as composition roots; move reusable behavior into focused hooks or helpers before the file becomes a god object.
- Derive view data during render when possible; do not mirror props or query data into local state unless the UI needs an editable draft.
- Use `useEffect` only for real synchronization with external systems, not for data derivation that could happen during render.
- When an effect needs the latest callback without resubscribing, prefer `useEffectEvent` over ref gymnastics.
- Use `startTransition` for non-urgent updates such as navigation-adjacent state, large filtering changes, or reconciliation that should not block input.
- Use `useDeferredValue` when a large derived view can lag slightly behind a typing or selection interaction.
- Do not add `useMemo` or `useCallback` by default. Add them only when referential stability is required by an API boundary or when profiling shows a real cost.
- Keep JSX branches readable. If a conditional block has its own responsibility, extract a component.
- Treat accessibility as part of composition quality: buttons stay buttons, dialogs manage focus, and interactive state must be reflected in semantics.

## Heuristics

- If a component owns networking, routing, dialogs, forms, and rendering details at once, split it.
- If local state only exists to cache something derivable, remove it.
- If an effect writes state after reading state from the same component, double-check whether the data should be derived instead.
- If a callback is passed deeply only to avoid one prop hop, consider whether the owning boundary is wrong before introducing more indirection.
