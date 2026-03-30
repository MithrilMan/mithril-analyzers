# Scriban Templates

## Goals

- Keep template rendering predictable and easy to maintain as the workspace contract evolves.
- Separate data shaping from template text.

## Best practices

- Build view models or render models before calling the template engine; do not assemble complex render data inline in the renderer.
- Keep template-specific models close to the rendering concern they support.
- Prefer many focused templates or focused model builders over one renderer that understands every scenario.
- Keep naming aligned between template fields and the render model to reduce hidden mapping logic.
- When a template contract changes, update its model builder and validation path in the same phase.
- Avoid leaking arbitrary domain services into template rendering code.

## Heuristics

- If the renderer class keeps growing new branches for unrelated template families, split it.
- If template data preparation requires many unrelated dependencies, the render model boundary is probably too broad.
