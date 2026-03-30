# CSS Modules

## Goals

- Keep styling local, intentional, and easy to reason about.
- Prevent global drift and utility-class sprawl.

## Best practices

- Scope styles to the component that owns the markup.
- Keep global CSS limited to reset, document defaults, theme tokens, and app-wide primitives.
- Prefer semantic class names tied to role or state, not presentational noise.
- Reuse shared tokens before introducing new raw values.
- Extract a shared UI primitive when several components repeat the same visual pattern and interaction semantics.
- Keep responsive behavior inside the owning module instead of layering global overrides.
- Avoid deeply nested selectors that hide ownership or make refactors brittle.
- Let markup structure stay readable; if the stylesheet requires excessive DOM nesting, reconsider the component split.

## Heuristics

- If styles are copied across modules, extract a component or shared primitive.
- If a module stylesheet is huge because it skins multiple responsibilities, split the component first.
