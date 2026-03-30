# Architecture

`Mithril.Analyzers` is organized as a small Roslyn-first workspace with a deliberately narrow V1 surface.

## Solution Layout

- `src/Mithril.Analyzers/`
  - one analyzer class per diagnostic
  - shared helpers for option lookup, generated-code detection, and line counting
- `tests/Mithril.Analyzers.Tests/`
  - Roslyn analyzer tests with inline source samples
- `docs/`
  - rule notes, migration guidance, and architecture references

## Packaging Model

- The NuGet package ships only analyzer assets under `analyzers/dotnet/cs`.
- The package does not expose runtime libraries to consumer applications.
- The package metadata points at this repository and carries the MIT license.

## Configuration Model

- Severity uses standard Roslyn `dotnet_diagnostic.<ID>.severity`.
- Numeric thresholds and booleans come from `.editorconfig` through `AnalyzerConfigOptionsProvider`.
- Glob-scoped sections are preferred for different thresholds in `src/`, `tests/`, and `tools/`.

## V1 Design Rules

- Prefer low-noise heuristics over clever but fragile analysis.
- Ignore generated code by default.
- Keep analyzer classes focused and helper utilities small enough to stay readable.
- Defer repository-specific architecture heuristics (`BGA100+`) until the maintainability baseline is proven in a real solution.
