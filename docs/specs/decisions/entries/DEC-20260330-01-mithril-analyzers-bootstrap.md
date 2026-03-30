# [DEC-20260330-01] Bootstrap Mithril.Analyzers as the project package and V1 scope

- Date: 2026-03-30
- Status: accepted
- Domain: architecture
- Keywords:
  - bootstrap
  - packaging
  - roslyn
  - editorconfig
- Supersedes: none
- Superseded-By: none

## Context

- The repository started as a generic template with no solution, no package metadata, and no project-specific operating constraints.
- The provided product spec requires a public MIT-licensed Roslyn analyzer package that replaces temporary maintainability guards with repository-owned diagnostics.
- The bootstrap needs to establish a durable package shape without overcommitting to V2 architecture heuristics before V1 parity exists.

## Options Considered

### Option A

- Create only an empty solution and postpone rules, packaging, and docs to later phases.
- Tradeoffs:
  - Lower immediate implementation effort.
  - Leaves the repository without a usable analyzer package or validation loop.

### Option B

- Bootstrap a real analyzer package with V1 diagnostics, `.editorconfig` configuration, tests, MIT licensing, and migration docs.
- Tradeoffs:
  - Higher upfront effort.
  - Produces an immediately usable foundation that can absorb V2 work safely.

## Decision

- Use `Mithril.Analyzers` as the package, assembly, and root namespace for the repository bootstrap.
- Ship the bootstrap as an analyzer-only NuGet package with `.editorconfig` as the single V1 configuration surface.
- Implement and document `BGA001` to `BGA005` now, while explicitly deferring `BGA100+` repository-informed rules until the V1 baseline is validated in real usage.

## Consequences

- The repository becomes a real OSS package rather than a generic template.
- Consumers can adopt the package with `PrivateAssets="all"` and no runtime dependency impact.
- The project documentation, AGENTS instructions, and note taxonomy now reflect analyzer-package work instead of template bootstrap steps.

## Validation / Evidence

- `docs/specs/backend-oss-analyzers-spec.md`
- `scripts/bootstrap-template.ps1`
- `Mithril.Analyzers.sln`
- `src/Mithril.Analyzers/Mithril.Analyzers.csproj`
- `tests/Mithril.Analyzers.Tests/Mithril.Analyzers.Tests.csproj`
