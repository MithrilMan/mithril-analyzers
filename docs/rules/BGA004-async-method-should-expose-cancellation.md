# BGA004 Async Method Should Expose Cancellation

## What It Matches

- source methods returning `Task`, `Task<T>`, `ValueTask`, or `ValueTask<T>` that expose no cancellation path

## What It Treats As Compliant

- a direct `CancellationToken` parameter
- a parameter type with a public `CancellationToken` property

## What It Ignores

- overrides
- interface implementations whose signature cannot be changed
- `Main`
- test methods when `mithril_analyzers.allow_missing_cancellation_for_tests = true`
- generated code

## Configuration

```ini
[*.cs]
mithril_analyzers.allow_missing_cancellation_for_tests = true
```

## Notes

- The heuristic is intentionally conservative to keep false positives actionable.
