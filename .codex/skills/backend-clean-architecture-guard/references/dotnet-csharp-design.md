# C# And .NET Backend Design

## Goals

- Keep backend code explicit, testable, and easy to evolve under change.
- Favor focused collaborators and clear invariants over broad utility services.

## Best practices

- Model the use case first, then place code in the smallest layer that can own it honestly.
- Prefer small classes with one reason to change and one dependency cluster.
- Keep pure transformations, normalization helpers, and invariant checks side-effect free when possible.
- Treat `async` as part of the contract: propagate cancellation tokens through IO paths and avoid fake async wrappers around synchronous work.
- Prefer immutable request/result models unless mutation is the real domain behavior.
- Use records or small dedicated types for value-like transport and option shapes when they improve clarity.
- Avoid static helper dumps; if logic has dependencies or domain meaning, give it an owning type.
- Keep naming intention-revealing: `Resolver`, `Builder`, `Normalizer`, `Store`, `Service`, `Writer`, and `Mapper` should match what the class actually does.
- When a service starts coordinating several sub-steps, extract collaborators instead of growing private-method clusters forever.

## Heuristics

- If a file needs a region map to be understandable, it probably wants decomposition.
- If a method both validates policy and performs IO, consider splitting policy from adapter work.
- If several call sites repeat the same null/shape normalization, centralize that behavior once at the boundary.
