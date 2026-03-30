# XYFlow / React Flow

Official sources consulted:

- https://reactflow.dev/

## Goals

- Keep canvas behavior smooth and predictable without leaking graph-engine details across the app.
- Separate graph data derivation from canvas viewport orchestration.

## Best practices

- Keep domain graph derivation outside the React Flow component when possible; the canvas should consume a prepared view model.
- Keep node and edge synchronization effects stable; callbacks used inside `setNodes`/`setEdges` flows should not churn accidentally.
- Store only the UI state that truly needs to be shared; ephemeral viewport behavior can stay widget-local.
- Treat selection, focus, minimap visibility, and viewport affordances as UI state, not domain state.
- Avoid triggering full canvas recomputation from unrelated UI changes.
- Use stable ids for nodes, edges, and repeated badges to avoid duplicate-key or reconciliation issues during live updates.
- Refit/recenter only when the user intent or the workflow policy justifies it; automatic viewport moves should be deliberate.

## Heuristics

- If a React Flow effect depends on many freshly recreated callbacks, check for avoidable rerender loops.
- If graph preparation and viewport choreography are mixed in one hook/component, split them before adding more behavior.
