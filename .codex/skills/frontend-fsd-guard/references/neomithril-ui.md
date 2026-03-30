# Neomithril UI

## Goals

- Use `@mithrilman/neomithril-ui` as the primary composition surface in the admin frontend.
- Avoid wrapping package primitives so heavily that the design system becomes invisible.

## Best practices

- Prefer package primitives and composition props before creating bespoke wrappers.
- Extract app-specific wrappers only when they add real behavior, repeated formatting, or durable domain semantics.
- Keep visual semantics aligned with the package vocabulary instead of restyling every primitive into a private mini-system.
- For deeper package usage guidance, prop discovery, or composition recipes, use the dedicated repository skills such as `use-neomithril-ui` or `compose-neomithril-ui`.

## Heuristics

- If a wrapper only renames props or forwards children plus class names, it is probably unnecessary.
- If several routes need the same higher-level composition pattern, extract one app-owned composite instead of many inconsistent wrappers.
