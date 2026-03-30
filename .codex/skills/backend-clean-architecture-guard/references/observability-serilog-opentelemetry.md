# Serilog And OpenTelemetry

## Goals

- Keep observability host-owned, structured, and proportionate to the runtime workflow.
- Make logs and telemetry useful for debugging long-lived backend workflows.

## Best practices

- Keep observability wiring in the host/composition layer, not scattered through feature services.
- Log structured data that helps reconstruct workflow state, not just prose.
- Prefer stable property names for recurring concepts such as job id, attempt id, provider, remote target, or operation kind.
- Use logs for narrative diagnostics and metrics/traces for flow visibility; do not force one signal to do all jobs.
- Avoid logging sensitive data or full payloads when identifiers and summarized context are enough.
- Keep telemetry configuration environment-driven and explicit.
- Emit errors where failures are handled or translated, not only at the deepest adapter layer.

## Heuristics

- If a class performs side effects but contributes no useful diagnostics, it may need clearer observability seams.
- If logs only say something failed without the owning identifiers, they will not help under load or recovery scenarios.
