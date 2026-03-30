# shadcn/ui

Official sources consulted:

- https://ui.shadcn.com/docs
- https://ui.shadcn.com/docs/new
- https://ui.shadcn.com/docs/components-json
- https://ui.shadcn.com/docs/theming

## Goals

- Treat shadcn/ui as owned application code, not as a black-box dependency to wrap defensively.
- Preserve composability, accessibility, and predictable APIs across the local primitive layer.
- Keep business semantics above `components/ui/`, while shared primitives stay generic and reusable.

## Best practices

- Remember the core model: shadcn/ui gives you the component source code, and your project owns that code.
- Prefer editing the owned primitive directly when a durable visual or behavioral change belongs to the component itself.
- Keep `components/ui/` business-agnostic. Domain-specific defaults, data shaping, and workflow semantics belong in higher layers.
- Reuse a common `cn()` helper and consistent variant patterns so the primitive layer stays predictable.
- When a primitive needs repeated visual modes, extend its local variant system instead of scattering ad-hoc `className` overrides everywhere.
- Preserve Radix accessibility semantics when composing dialogs, selects, tabs, popovers, and other interactive primitives.
- Keep component APIs composable and boring: standard props, variant props, and `className` extension points should remain easy to understand.
- Do not cargo-cult every registry component. Bring in or adapt only the primitives the project actually needs.
- When theming is token-driven, keep the bridge between CSS variables and Tailwind utilities aligned with the local Tailwind theme strategy.
- If the project uses “shadcn-style” local primitives without the CLI or official registry, still follow the same ownership and composition rules.

## Heuristics

- If a wrapper only forwards props and re-skins one primitive for one route, it probably belongs as a `className` or variant change in the owned primitive instead.
- If several features need the same behavioral composition over primitives, extract a shared app-level composite above `components/ui/`.
- If a primitive file starts accumulating domain language, move that domain concern out and keep the primitive generic.
- If a change would break Radix title/description/focus expectations, fix the semantics before polishing the visuals.
