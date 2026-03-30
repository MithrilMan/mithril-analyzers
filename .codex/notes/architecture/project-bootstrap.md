# Project Bootstrap

- Status: active
- Confidence: high
- Last-Validated: 2026-03-30
- Evidence: Mithril.Analyzers.sln; src/Mithril.Analyzers/Mithril.Analyzers.csproj; docs/specs/decisions/entries/DEC-20260330-01-mithril-analyzers-bootstrap.md
- Related: .codex/notes/rules/v1-diagnostic-scope.md
- Supersedes: none
- Superseded-By: none

## Facts

- The repository is now bootstrapped as the `Mithril.Analyzers` Roslyn package with an analyzer-only NuGet output.
- V1 is intentionally limited to `BGA001` through `BGA005`, with configuration sourced from `.editorconfig`.
- The solution uses `src/Mithril.Analyzers/` for production code and `tests/Mithril.Analyzers.Tests/` for validation.

## Implications

- Future work should preserve the analyzer-only packaging model and avoid introducing runtime assets.
- `BGA100+` rules should land in follow-up phases after the maintainability baseline is proven.

## Follow-ups

- Revisit package publishing metadata and CI once the first public release flow is added.
