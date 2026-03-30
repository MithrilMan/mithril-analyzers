# Background Jobs And Hosted Services

## Goals

- Keep asynchronous workflow execution observable, cancellable, and easy to reason about.
- Separate orchestration from adapter concerns.

## Best practices

- Keep hosted workers small and focused on dequeue/dispatch loops; push workflow policy into collaborators.
- Make cancellation explicit and propagate `CancellationToken` through queues, process runners, and long-running IO.
- Prefer narrow command/result contracts between orchestrators and runners.
- Keep retries, recovery, and fallback policy visible instead of hiding them inside low-level adapters.
- Distinguish coordination services from execution adapters: queueing, lifecycle transitions, remote execution, and process launching rarely belong in the same class.
- Emit enough structured events or logs that long-running background behavior can be reconstructed later.
- When process execution is involved, isolate command building, environment shaping, and result mapping into separate seams before the runner grows too large.

## Heuristics

- If a hosted service is doing queueing, persistence, retries, status transitions, and event projection itself, it needs collaborators.
- If cancellation or shutdown behavior is hard to explain, the orchestration boundary is probably too blurry.
